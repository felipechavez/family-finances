namespace FinanceApp.Application.Features.Transactions.DeleteTransaction;
using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="DeleteTransactionCommand"/>: removes a transaction and reverses its effect on the account balance.
/// Enforces family ownership to prevent cross-family deletion.
/// </summary>
public class DeleteTransactionHandler(Client supabase, ILogger<DeleteTransactionHandler> logger)
    : IRequestHandler<DeleteTransactionCommand>
{
    public async Task Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var existing = await supabase.From<Transaction>()
            .Where(t => t.Id == request.TransactionId && t.FamilyId == request.FamilyId)
            .Get();

        var tx = existing.Model
            ?? throw new AppException(LocalizationKeys.Transaction_NotFound, 404, request.TransactionId);

        await supabase.From<Transaction>()
            .Where(t => t.Id == request.TransactionId && t.FamilyId == request.FamilyId)
            .Delete();

        // Revertir el efecto en el balance de forma atómica (inverso de la creación).
        var delta = tx.Type == TransactionType.Income ? -tx.Amount : tx.Amount;
        await supabase.Rpc("adjust_account_balance",
            new Dictionary<string, object> { ["p_account_id"] = tx.AccountId, ["p_delta"] = delta });

        logger.LogInformation(
            "Transaction {TransactionId} deleted; account {AccountId} balance adjusted by {Delta}",
            request.TransactionId, tx.AccountId, delta);
    }
}
