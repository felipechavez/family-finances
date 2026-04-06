namespace FinanceApp.Application.Features.Budgets.GetBudgets;
using FinanceApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Handles <see cref="GetBudgetsQuery"/>: retrieves all budget limits defined for a family.
/// </summary>
public class GetBudgetsHandler(IAppDbContext db) : IRequestHandler<GetBudgetsQuery, IReadOnlyList<BudgetDto>>
{
    /// <inheritdoc/>
    public async Task<IReadOnlyList<BudgetDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
        => await db.Budgets
            .AsNoTracking()
            .Where(b => b.FamilyId == request.FamilyId)
            .Join(db.Categories, b => b.CategoryId, c => c.Id,
                (b, c) => new BudgetDto(b.Id, b.FamilyId, b.CategoryId, c.Name, b.MonthlyLimit))
            .ToListAsync(cancellationToken);
}
