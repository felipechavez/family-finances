namespace FinanceApp.Application.Features.Transactions.DeleteTransaction;
using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="DeleteTransactionCommand"/>: removes a transaction from the database.
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

        logger.LogInformation("Transaction {TransactionId} deleted from family {FamilyId}",
            request.TransactionId, request.FamilyId);
    }
}
