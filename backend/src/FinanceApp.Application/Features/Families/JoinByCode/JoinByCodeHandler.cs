namespace FinanceApp.Application.Features.Families.JoinByCode;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Application.Features.Families;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;
using static Supabase.Postgrest.Constants;

/// <summary>
/// Handles <see cref="JoinByCodeCommand"/>: looks up the family by its invite code,
/// adds the user as a member, and issues a new JWT with the embedded family_id claim.
/// </summary>
public class JoinByCodeHandler(Client supabase, IJwtService jwt, ILogger<JoinByCodeHandler> logger)
    : IRequestHandler<JoinByCodeCommand, FamilySetupResult>
{
    public async Task<FamilySetupResult> Handle(JoinByCodeCommand request, CancellationToken cancellationToken)
    {
        // Look up family by invite code (case-insensitive: store upper, compare upper)
        var familyResp = await supabase.From<Family>()
            .Filter("invite_code", Operator.Equals, request.Code.ToUpperInvariant())
            .Get();

        var family = familyResp.Model
            ?? throw new AppException(LocalizationKeys.Family_InvalidInviteCode, 404);

        // Idempotent: skip insert if already a member
        var existingResp = await supabase.From<FamilyMember>()
            .Where(m => m.UserId == request.UserId && m.FamilyId == family.Id)
            .Get();

        if (existingResp.Model == null)
        {
            var member = FamilyMember.Create(family.Id, request.UserId, FamilyRole.Member);
            await supabase.From<FamilyMember>().Insert(member);
        }

        var userResp = await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Get();
        var user = userResp.Model!;

        var token = jwt.GenerateToken(user, family.Id, FamilyRole.Member.ToString());

        logger.LogInformation("User {UserId} joined family {FamilyId} via invite code", request.UserId, family.Id);

        return new FamilySetupResult(token, family.Id);
    }
}
