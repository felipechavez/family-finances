namespace FinanceApp.Application.Features.Auth.Register;
using FluentValidation;
using FinanceApp.Application.Common.Validation;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        PasswordRules.Apply(RuleFor(x => x.Password).NotEmpty());
    }
}
