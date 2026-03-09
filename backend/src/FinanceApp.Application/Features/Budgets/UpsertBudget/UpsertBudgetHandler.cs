namespace FinanceApp.Application.Features.Budgets.UpsertBudget;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class UpsertBudgetHandler(IAppDbContext db) : IRequestHandler<UpsertBudgetCommand, Guid>
{
    public async Task<Guid> Handle(UpsertBudgetCommand request, CancellationToken cancellationToken)
    {
        var existing = await db.Budgets
            .FirstOrDefaultAsync(b => b.FamilyId == request.FamilyId && b.CategoryId == request.CategoryId,
                cancellationToken);

        if (existing is not null)
        {
            existing.UpdateLimit(request.MonthlyLimit);
        }
        else
        {
            existing = Budget.Create(request.FamilyId, request.CategoryId, request.MonthlyLimit);
            db.Budgets.Add(existing);
        }

        await db.SaveChangesAsync(cancellationToken);
        return existing.Id;
    }
}
