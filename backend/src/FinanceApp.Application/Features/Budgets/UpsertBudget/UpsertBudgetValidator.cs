namespace FinanceApp.Application.Features.Budgets.UpsertBudget;
using FluentValidation;

public class UpsertBudgetValidator : AbstractValidator<UpsertBudgetCommand>
{
    public UpsertBudgetValidator()
    {
        RuleFor(x => x.FamilyId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.MonthlyLimit).GreaterThanOrEqualTo(0);
    }
}
