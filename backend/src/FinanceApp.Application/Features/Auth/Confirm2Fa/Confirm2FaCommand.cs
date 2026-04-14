namespace FinanceApp.Application.Features.Auth.Confirm2Fa;
using MediatR;

/// <summary>Command to confirm 2FA setup by verifying the first TOTP code, enabling 2FA for the account.</summary>
/// <param name="UserId">The authenticated user confirming 2FA.</param>
/// <param name="Code">The 6-digit TOTP code from the authenticator app.</param>
public record Confirm2FaCommand(Guid UserId, string Code) : IRequest;
