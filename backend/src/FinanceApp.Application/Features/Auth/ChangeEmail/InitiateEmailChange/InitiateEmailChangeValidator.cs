namespace FinanceApp.Application.Features.Auth.ChangeEmail.InitiateEmailChange;
using FluentValidation;

public class InitiateEmailChangeValidator : AbstractValidator<InitiateEmailChangeCommand>
{
    public InitiateEmailChangeValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);
    }
}
