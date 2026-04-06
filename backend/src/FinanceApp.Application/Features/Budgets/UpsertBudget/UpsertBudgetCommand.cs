namespace FinanceApp.Application.Features.Budgets.UpsertBudget;
using MediatR;

/// <summary>
/// Command to create or update the monthly budget limit for a category within a family.
/// If a budget already exists for the given family/category combination, its limit is updated.
/// </summary>
/// <param name="FamilyId">The owning family's identifier.</param>
/// <param name="CategoryId">The category to budget for.</param>
/// <param name="MonthlyLimit">The new monthly spending limit. Must be non-negative.</param>
public record UpsertBudgetCommand(Guid FamilyId, Guid CategoryId, decimal MonthlyLimit) : IRequest<Guid>;
