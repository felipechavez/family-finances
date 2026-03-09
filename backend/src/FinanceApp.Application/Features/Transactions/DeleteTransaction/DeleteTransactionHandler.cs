namespace FinanceApp.Application.Features.Transactions.DeleteTransaction;
using FinanceApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteTransactionHandler(IAppDbContext db) : IRequestHandler<DeleteTransactionCommand>
{
    public async Task Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = await db.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.TransactionId && t.FamilyId == request.FamilyId,
                cancellationToken)
            ?? throw new KeyNotFoundException($"Transaction {request.TransactionId} not found.");

        db.Transactions.Remove(tx);
        await db.SaveChangesAsync(cancellationToken);
    }
}
