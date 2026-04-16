namespace FinanceApp.Application.Features.Families.JoinByCode;
using FluentValidation;

public class JoinByCodeValidator : AbstractValidator<JoinByCodeCommand>
{
    public JoinByCodeValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(6, 10)
            .Matches("^[A-Z0-9]+$").WithMessage("Invite code must be uppercase alphanumeric.");

        RuleFor(x => x.UserId).NotEmpty();
    }
}
