namespace FinanceApp.Application.Features.Budgets.UpsertBudget;
using MediatR;

public record UpsertBudgetCommand(Guid FamilyId, Guid CategoryId, decimal MonthlyLimit) : IRequest<Guid>;
