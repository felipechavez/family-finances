namespace FinanceApp.Application.Features.Families.JoinByCode;
using FinanceApp.Application.Features.Families;
using MediatR;

/// <summary>Command to join a family using its short 8-character invite code.</summary>
public record JoinByCodeCommand(string Code, Guid UserId) : IRequest<FamilySetupResult>;
