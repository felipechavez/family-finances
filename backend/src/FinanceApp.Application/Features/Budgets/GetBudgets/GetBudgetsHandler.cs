namespace FinanceApp.Application.Features.Budgets.GetBudgets;
using FinanceApp.Domain.Entities;
using MediatR;
using Supabase;

/// <summary>
/// Handles <see cref="GetBudgetsQuery"/>: retrieves all budget limits defined for a family.
/// </summary>
public class GetBudgetsHandler(Client supabase) : IRequestHandler<GetBudgetsQuery, IReadOnlyList<BudgetDto>>
{
    public async Task<IReadOnlyList<BudgetDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgetsResponse = await supabase.From<Budget>()
            .Filter("family_id", Supabase.Postgrest.Constants.Operator.Equals, request.FamilyId.ToString())
            .Get();

        var budgets = budgetsResponse.Models ?? new List<Budget>();
        var categoryIds = budgets.Select(b => b.CategoryId).Distinct().ToList();

        var categoriesResponse = categoryIds.Any()
            ? await supabase.From<Category>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, categoryIds.Select(id => id.ToString()).ToList())
                .Get()
            : null;

        var categories = categoriesResponse?.Models?.ToDictionary(c => c.Id, c => c.Name)
            ?? new Dictionary<Guid, string>();

        return budgets
            .Select(b => new BudgetDto(
                b.Id,
                b.FamilyId,
                b.CategoryId,
                categories.GetValueOrDefault(b.CategoryId, "Unknown"),
                b.MonthlyLimit))
            .ToList();
    }
}
