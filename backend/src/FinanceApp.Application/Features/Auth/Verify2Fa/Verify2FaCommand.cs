namespace FinanceApp.Application.Features.Auth.Verify2Fa;
using FinanceApp.Application.Features.Auth.Login;
using MediatR;

/// <summary>Command to complete the 2FA login step and receive a full JWT token.</summary>
/// <param name="ChallengeToken">The short-lived challenge token returned during the initial login.</param>
/// <param name="Code">The 6-digit TOTP code from the authenticator app.</param>
public record Verify2FaCommand(string ChallengeToken, string Code) : IRequest<LoginResult>;
