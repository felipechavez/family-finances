namespace FinanceApp.Application.Features.Families.JoinFamily;
using FluentValidation;

public class JoinFamilyValidator : AbstractValidator<JoinFamilyCommand>
{
    public JoinFamilyValidator()
    {
        RuleFor(x => x.FamilyId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
