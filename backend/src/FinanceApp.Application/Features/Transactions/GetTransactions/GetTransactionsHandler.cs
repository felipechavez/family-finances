namespace FinanceApp.Application.Features.Transactions.GetTransactions;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Application.Features.Transactions.CreateTransaction;
using MediatR;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Handles <see cref="GetTransactionsQuery"/>: retrieves transactions for a family with optional month/year filtering.
/// </summary>
public class GetTransactionsHandler(IAppDbContext db)
    : IRequestHandler<GetTransactionsQuery, IReadOnlyList<TransactionDto>>
{
    /// <inheritdoc/>
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
            .Join(db.Categories, t => t.CategoryId, c => c.Id,
                (t, c) => new TransactionDto(
                    t.Id,
                    t.FamilyId,
                    t.AccountId,
                    t.UserId,
                    t.CategoryId,
                    c.Name,
                    t.Type.ToString(),
                    t.Amount,
                    t.Currency,
                    t.Description,
                    t.TransactionDate,
                    t.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
