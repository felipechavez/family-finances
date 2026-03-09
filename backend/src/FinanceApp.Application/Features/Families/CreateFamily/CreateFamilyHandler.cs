namespace FinanceApp.Application.Features.Families.CreateFamily;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;

public class CreateFamilyHandler(IAppDbContext db) : IRequestHandler<CreateFamilyCommand, Guid>
{
    public async Task<Guid> Handle(CreateFamilyCommand request, CancellationToken cancellationToken)
    {
        var family = Family.Create(request.Name, request.OwnerUserId);
        family.AddMember(request.OwnerUserId, FamilyRole.Owner);
        db.Families.Add(family);
        await db.SaveChangesAsync(cancellationToken);
        return family.Id;
    }
}
