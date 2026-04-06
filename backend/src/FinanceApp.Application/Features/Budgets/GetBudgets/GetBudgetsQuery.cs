namespace FinanceApp.Application.Features.Budgets.GetBudgets;
using MediatR;

/// <summary>Query to retrieve all budget limits defined for a family.</summary>
/// <param name="FamilyId">The family whose budgets to retrieve.</param>
public record GetBudgetsQuery(Guid FamilyId) : IRequest<IReadOnlyList<BudgetDto>>;

/// <summary>Projection of a <see cref="FinanceApp.Domain.Entities.Budget"/> for API responses.</summary>
/// <param name="Id">The budget's unique identifier.</param>
/// <param name="FamilyId">The owning family's identifier.</param>
/// <param name="CategoryId">The category this budget applies to.</param>
/// <param name="CategoryName">The display name of the category.</param>
/// <param name="MonthlyLimit">The maximum allowed spending per month.</param>
public record BudgetDto(Guid Id, Guid FamilyId, Guid CategoryId, string CategoryName, decimal MonthlyLimit);
