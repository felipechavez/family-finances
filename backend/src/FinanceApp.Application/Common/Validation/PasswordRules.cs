namespace FinanceApp.Application.Common.Validation;
using FluentValidation;

public static class PasswordRules
{
    public static IRuleBuilderOptions<T, string> Apply<T>(IRuleBuilder<T, string> rule) =>
        rule
            .MinimumLength(8).WithErrorCode(LocalizationKeys.Password_RequiresUppercase)
            .MaximumLength(100)
            .Matches("[A-Z]").WithMessage(LocalizationKeys.Password_RequiresUppercase)
            .Matches("[a-z]").WithMessage(LocalizationKeys.Password_RequiresLowercase)
            .Matches("[0-9]").WithMessage(LocalizationKeys.Password_RequiresDigit)
            .Matches(@"[!@#$%^&*()_\-+=\[\]{};':""\\|,.<>\/?`~]").WithMessage(LocalizationKeys.Password_RequiresSpecial);
}
