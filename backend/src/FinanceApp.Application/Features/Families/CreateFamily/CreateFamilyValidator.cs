namespace FinanceApp.Application.Features.Families.CreateFamily;
using FluentValidation;

public class CreateFamilyValidator : AbstractValidator<CreateFamilyCommand>
{
    public CreateFamilyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.OwnerUserId).NotEmpty();
    }
}
