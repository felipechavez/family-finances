namespace FinanceApp.Application.Features.Families.JoinFamily;
using FinanceApp.Application.Features.Families;
using MediatR;

/// <summary>Command to add an existing user as a member of an existing family.</summary>
/// <param name="FamilyId">The family to join.</param>
/// <param name="UserId">The user joining (set from JWT claims).</param>
public record JoinFamilyCommand(Guid FamilyId, Guid UserId) : IRequest<FamilySetupResult>;
