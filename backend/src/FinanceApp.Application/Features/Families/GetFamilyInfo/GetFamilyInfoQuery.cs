namespace FinanceApp.Application.Features.Families.GetFamilyInfo;
using MediatR;

/// <summary>Returns the current family's details: name, invite code, and member list.</summary>
public record GetFamilyInfoQuery(Guid FamilyId, Guid UserId) : IRequest<FamilyInfoResult>;

/// <param name="FamilyId">Identifier of the family.</param>
/// <param name="Name">Display name of the family.</param>
/// <param name="InviteCode">Short 8-char code that other users can enter to join.</param>
/// <param name="IsOwner">True when the requesting user is the family owner.</param>
/// <param name="Members">All current members of the family.</param>
public record FamilyInfoResult(
    Guid FamilyId,
    string Name,
    string InviteCode,
    bool IsOwner,
    IReadOnlyList<FamilyMemberDto> Members);

/// <param name="UserId">Member's user identifier.</param>
/// <param name="Name">Member's display name.</param>
/// <param name="Role">Role within the family (owner / admin / member).</param>
/// <param name="JoinedAt">UTC timestamp when the member joined.</param>
public record FamilyMemberDto(Guid UserId, string Name, string Role, DateTime JoinedAt);
