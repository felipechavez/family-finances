namespace FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="CreateTransactionCommand"/>: persists a new transaction, adjusts the account
/// balance, and triggers a budget-exceeded notification + email when applicable.
/// </summary>
public class CreateTransactionHandler(
    Client supabase,
    INotificationService notifications,
    IEmailService emailService,
    ILogger<CreateTransactionHandler> logger)
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = Transaction.Create(
            request.FamilyId, request.AccountId, request.UserId,
            request.CategoryId, request.Type, request.Amount,
            request.Description, request.TransactionDate, request.Currency);

        await supabase.From<Transaction>().Insert(tx);

        // Ajuste atómico del balance: evita read-modify-write y condiciones de carrera.
        var delta = tx.Type == TransactionType.Income ? tx.Amount : -tx.Amount;
        await supabase.Rpc("adjust_account_balance",
            new Dictionary<string, object> { ["p_account_id"] = tx.AccountId, ["p_delta"] = delta });

        var categoryResponse = await supabase.From<Category>()
            .Where(c => c.Id == tx.CategoryId)
            .Get();

        var categoryName = categoryResponse.Model?.Name
            ?? throw new AppException(LocalizationKeys.Transaction_CategoryNotFound, 404, tx.CategoryId);

        logger.LogInformation("Transaction {TransactionId} ({Type} {Amount}) created for family {FamilyId}",
            tx.Id, tx.Type, tx.Amount, request.FamilyId);

        // ── Budget exceeded check (expenses only) ─────────────────────────────
        if (tx.Type == TransactionType.Expense)
            await CheckBudgetAsync(tx, categoryName, request.UserId, cancellationToken);

        return ToDto(tx, categoryName);
    }

    private async Task CheckBudgetAsync(
        Transaction tx, string categoryName, Guid userId, CancellationToken ct)
    {
        try
        {
            // 1. Get budget for this category+family
            var budgetResp = await supabase.From<Budget>()
                .Where(b => b.FamilyId == tx.FamilyId && b.CategoryId == tx.CategoryId)
                .Get();

            var budget = budgetResp.Model;
            if (budget is null || budget.MonthlyLimit <= 0) return;

            // 2. Sum expenses this month for this category+family
            var monthStart = new DateOnly(tx.TransactionDate.Year, tx.TransactionDate.Month, 1);
            var monthEnd   = monthStart.AddMonths(1);

            var txResp = await supabase.From<Transaction>()
                .Where(t => t.FamilyId == tx.FamilyId
                         && t.CategoryId == tx.CategoryId
                         && t.Type == TransactionType.Expense
                         && t.TransactionDate >= monthStart
                         && t.TransactionDate < monthEnd)
                .Get();

            var totalSpent = txResp.Models?.Sum(t => t.Amount) ?? 0m;

            if (totalSpent <= budget.MonthlyLimit) return;

            // 3. Create in-app notification
            await notifications.CreateBudgetExceededAsync(
                userId, tx.FamilyId, categoryName, totalSpent, budget.MonthlyLimit, ct);

            // 4. Send email alert (fire-and-forget — failure must not break the transaction)
            var userResp = await supabase.From<Users>().Where(u => u.Id == userId).Get();
            if (userResp.Model is { } user)
            {
                _ = emailService.SendBudgetAlertAsync(
                    user.Email, user.Name, categoryName, totalSpent, budget.MonthlyLimit, ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Budget check failed for transaction {TxId}", tx.Id);
        }
    }

    internal static TransactionDto ToDto(Transaction tx, string categoryName) => new(
        tx.Id, tx.FamilyId, tx.AccountId, tx.UserId, tx.CategoryId,
        categoryName, tx.Type.ToString(), tx.Amount, tx.Currency, tx.Description,
        tx.TransactionDate, tx.CreatedAt);
}
