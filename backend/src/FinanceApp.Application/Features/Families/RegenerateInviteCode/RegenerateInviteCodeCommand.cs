namespace FinanceApp.Application.Features.Families.RegenerateInviteCode;
using MediatR;

/// <summary>Command to regenerate the invite code of a family. Only the owner may do this.</summary>
public record RegenerateInviteCodeCommand(Guid FamilyId, Guid UserId) : IRequest<string>;
