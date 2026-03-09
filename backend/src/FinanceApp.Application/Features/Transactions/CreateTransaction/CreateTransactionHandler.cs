namespace FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using MediatR;

public class CreateTransactionHandler(IAppDbContext db) : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = Transaction.Create(
            request.FamilyId, request.AccountId, request.UserId,
            request.CategoryId, request.Type, request.Amount,
            request.Description, request.TransactionDate, request.Currency);

        db.Transactions.Add(tx);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(tx);
    }

    internal static TransactionDto ToDto(Transaction tx) => new(
        tx.Id, tx.FamilyId, tx.AccountId, tx.UserId, tx.CategoryId,
        tx.Type.ToString(), tx.Amount, tx.Currency, tx.Description,
        tx.TransactionDate, tx.CreatedAt);
}
