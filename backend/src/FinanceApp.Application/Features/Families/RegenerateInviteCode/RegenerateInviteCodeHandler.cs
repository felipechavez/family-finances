namespace FinanceApp.Application.Features.Families.RegenerateInviteCode;
using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="RegenerateInviteCodeCommand"/>: verifies the requesting user is the family
/// owner, generates a new invite code, persists it, and returns the new code.
/// </summary>
public class RegenerateInviteCodeHandler(Client supabase, ILogger<RegenerateInviteCodeHandler> logger)
    : IRequestHandler<RegenerateInviteCodeCommand, string>
{
    public async Task<string> Handle(RegenerateInviteCodeCommand request, CancellationToken cancellationToken)
    {
        var familyResp = await supabase.From<Family>()
            .Where(f => f.Id == request.FamilyId)
            .Get();

        var family = familyResp.Model
            ?? throw new AppException(LocalizationKeys.Family_NotFound, 404);

        if (family.OwnerUserId != request.UserId)
            throw new AppException(LocalizationKeys.Family_NotOwner, 403);

        var newCode = Family.GenerateCode();
        family.InviteCode = newCode;

        await supabase.From<Family>()
            .Where(f => f.Id == family.Id)
            .Update(family);

        logger.LogInformation("Invite code regenerated for family {FamilyId} by owner {UserId}", family.Id, request.UserId);

        return newCode;
    }
}
