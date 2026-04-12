namespace FinanceApp.Application.Features.Budgets.UpsertBudget;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="UpsertBudgetCommand"/>: creates a new budget or updates the limit of an existing one.
/// </summary>
public class UpsertBudgetHandler(Client supabase, ILogger<UpsertBudgetHandler> logger)
    : IRequestHandler<UpsertBudgetCommand, Guid>
{
    public async Task<Guid> Handle(UpsertBudgetCommand request, CancellationToken cancellationToken)
    {
        var existingResponse = await supabase.From<Budget>()
            .Where(b => b.FamilyId == request.FamilyId && b.CategoryId == request.CategoryId)
            .Get();

        var existing = existingResponse.Model;

        if (existing is not null)
        {
            existing.UpdateLimit(request.MonthlyLimit);
            await supabase.From<Budget>()
                .Where(b => b.Id == existing.Id)
                .Update(existing);

            logger.LogInformation("Budget {BudgetId} updated for family {FamilyId}, category {CategoryId}",
                existing.Id, request.FamilyId, request.CategoryId);
        }
        else
        {
            existing = Budget.Create(request.FamilyId, request.CategoryId, request.MonthlyLimit);
            await supabase.From<Budget>().Insert(existing);

            logger.LogInformation("Budget {BudgetId} created for family {FamilyId}, category {CategoryId}",
                existing.Id, request.FamilyId, request.CategoryId);
        }

        return existing.Id;
    }
}
