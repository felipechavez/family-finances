namespace FinanceApp.Application.Features.Transactions.DeleteTransaction;
using FinanceApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handles <see cref="DeleteTransactionCommand"/>: removes a transaction from the database.
/// Enforces family ownership to prevent cross-family deletion.
/// </summary>
public class DeleteTransactionHandler(IAppDbContext db, ILogger<DeleteTransactionHandler> logger)
    : IRequestHandler<DeleteTransactionCommand>
{
    /// <inheritdoc/>
    public async Task Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = await db.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.TransactionId && t.FamilyId == request.FamilyId,
                cancellationToken)
            ?? throw new KeyNotFoundException($"Transaction {request.TransactionId} not found.");

        db.Transactions.Remove(tx);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Transaction {TransactionId} deleted from family {FamilyId}",
            request.TransactionId, request.FamilyId);
    }
}
