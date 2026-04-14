namespace FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="CreateTransactionCommand"/>: persists a new transaction and returns its projection.
/// </summary>
public class CreateTransactionHandler(Client supabase, ILogger<CreateTransactionHandler> logger)
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

        return ToDto(tx, categoryName);
    }

    internal static TransactionDto ToDto(Transaction tx, string categoryName) => new(
        tx.Id, tx.FamilyId, tx.AccountId, tx.UserId, tx.CategoryId,
        categoryName, tx.Type.ToString(), tx.Amount, tx.Currency, tx.Description,
        tx.TransactionDate, tx.CreatedAt);
}
