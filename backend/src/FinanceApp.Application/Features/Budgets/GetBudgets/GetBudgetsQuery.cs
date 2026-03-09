namespace FinanceApp.Application.Features.Budgets.GetBudgets;
using MediatR;

public record GetBudgetsQuery(Guid FamilyId) : IRequest<IReadOnlyList<BudgetDto>>;
public record BudgetDto(Guid Id, Guid FamilyId, Guid CategoryId, string CategoryName, decimal MonthlyLimit);
