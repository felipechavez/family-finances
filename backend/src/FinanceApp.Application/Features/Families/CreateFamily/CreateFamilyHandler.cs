namespace FinanceApp.Application.Features.Families.CreateFamily;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Application.Features.Families;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="CreateFamilyCommand"/>: creates a new family group, registers its owner as the
/// first member, and issues a new JWT that embeds the <c>family_id</c> claim.
/// </summary>
public class CreateFamilyHandler(Client supabase, IJwtService jwt, ILogger<CreateFamilyHandler> logger)
    : IRequestHandler<CreateFamilyCommand, FamilySetupResult>
{
    public async Task<FamilySetupResult> Handle(CreateFamilyCommand request, CancellationToken cancellationToken)
    {
        var family = Family.Create(request.Name, request.OwnerUserId);
        await supabase.From<Family>().Insert(family);

        var member = FamilyMember.Create(family.Id, request.OwnerUserId, FamilyRole.Owner);
        await supabase.From<FamilyMember>().Insert(member);

        var userResponse = await supabase.From<Users>()
            .Where(u => u.Id == request.OwnerUserId)
            .Get();
        var user = userResponse.Model!;

        var token = jwt.GenerateToken(user, family.Id, FamilyRole.Owner.ToString());

        logger.LogInformation("Family {FamilyId} created by user {UserId}", family.Id, request.OwnerUserId);

        return new FamilySetupResult(token, family.Id);
    }
}
