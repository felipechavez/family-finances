namespace FinanceApp.Application.Features.Transactions.GetTransactions;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Application.Features.Transactions.CreateTransaction;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetTransactionsHandler(IAppDbContext db)
    : IRequestHandler<GetTransactionsQuery, IReadOnlyList<TransactionDto>>
{
    public async Task<IReadOnlyList<TransactionDto>> Handle(
        GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Transactions
            .AsNoTracking()
            .Where(t => t.FamilyId == request.FamilyId);

        if (request.Year.HasValue)
            query = query.Where(t => t.TransactionDate.Year == request.Year.Value);
        if (request.Month.HasValue)
            query = query.Where(t => t.TransactionDate.Month == request.Month.Value);

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => CreateTransactionHandler.ToDto(t))
            .ToListAsync(cancellationToken);
    }
}
