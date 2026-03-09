namespace FinanceApp.Application.Features.Accounts.CreateAccount;
using FluentValidation;

public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.InitialBalance).GreaterThanOrEqualTo(0);
    }
}
