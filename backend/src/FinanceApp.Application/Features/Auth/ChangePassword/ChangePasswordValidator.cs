namespace FinanceApp.Application.Features.Auth.ChangePassword;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Validation;
using FluentValidation;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        PasswordRules.Apply(RuleFor(x => x.NewPassword));
        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage(LocalizationKeys.Auth_PasswordMismatch);
    }
}
