namespace FinanceApp.Application.Features.Families.GetFamilyInfo;
using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Supabase;

/// <summary>
/// Handles <see cref="GetFamilyInfoQuery"/>: loads the family record, its members, and each
/// member's user display name, then returns a combined DTO.
/// </summary>
public class GetFamilyInfoHandler(Client supabase)
    : IRequestHandler<GetFamilyInfoQuery, FamilyInfoResult>
{
    public async Task<FamilyInfoResult> Handle(GetFamilyInfoQuery request, CancellationToken cancellationToken)
    {
        // 1. Load the family
        var familyResp = await supabase.From<Family>()
            .Where(f => f.Id == request.FamilyId)
            .Get();
        var family = familyResp.Model
            ?? throw new AppException(LocalizationKeys.Family_NotFound, 404);

        // 2. Load all members for this family
        var membersResp = await supabase.From<FamilyMember>()
            .Where(m => m.FamilyId == request.FamilyId)
            .Get();
        var members = membersResp.Models;

        // 3. Load the corresponding users in one query (filter by id IN list)
        var userIds = members.Select(m => m.UserId).ToList();
        var usersResp = await supabase.From<Users>().Get();
        var userMap = usersResp.Models
            .Where(u => userIds.Contains(u.Id))
            .ToDictionary(u => u.Id, u => u.Name);

        var memberDtos = members
            .Select(m => new FamilyMemberDto(
                m.UserId,
                userMap.GetValueOrDefault(m.UserId, "—"),
                m.Role.ToString().ToLowerInvariant(),
                m.JoinedAt))
            .OrderBy(m => m.Role == FamilyRole.Owner.ToString().ToLowerInvariant() ? 0 : 1)
            .ThenBy(m => m.JoinedAt)
            .ToList();

        var isOwner = family.OwnerUserId == request.UserId;

        return new FamilyInfoResult(
            family.Id,
            family.Name,
            family.InviteCode,
            isOwner,
            memberDtos);
    }
}
