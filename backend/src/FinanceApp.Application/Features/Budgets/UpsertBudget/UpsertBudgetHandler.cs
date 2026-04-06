namespace FinanceApp.Application.Features.Budgets.UpsertBudget;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handles <see cref="UpsertBudgetCommand"/>: creates a new budget or updates the limit of an existing one.
/// </summary>
public class UpsertBudgetHandler(IAppDbContext db, ILogger<UpsertBudgetHandler> logger)
    : IRequestHandler<UpsertBudgetCommand, Guid>
{
    /// <inheritdoc/>
    public async Task<Guid> Handle(UpsertBudgetCommand request, CancellationToken cancellationToken)
    {
        var existing = await db.Budgets
            .FirstOrDefaultAsync(b => b.FamilyId == request.FamilyId && b.CategoryId == request.CategoryId,
                cancellationToken);

        if (existing is not null)
        {
            existing.UpdateLimit(request.MonthlyLimit);
            logger.LogInformation("Budget {BudgetId} updated for family {FamilyId}, category {CategoryId}",
                existing.Id, request.FamilyId, request.CategoryId);
        }
        else
        {
            existing = Budget.Create(request.FamilyId, request.CategoryId, request.MonthlyLimit);
            db.Budgets.Add(existing);
            logger.LogInformation("Budget {BudgetId} created for family {FamilyId}, category {CategoryId}",
                existing.Id, request.FamilyId, request.CategoryId);
        }

        await db.SaveChangesAsync(cancellationToken);
        return existing.Id;
    }
}
