namespace FinanceApp.Application.Features.Accounts.UpdateAccount;
using FluentValidation;

public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
    }
}
