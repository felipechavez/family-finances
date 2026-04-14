namespace FinanceApp.Application.Features.Auth.Disable2Fa;
using MediatR;

/// <summary>Command to disable 2FA on an account. Requires the current TOTP code as confirmation.</summary>
/// <param name="UserId">The authenticated user disabling 2FA.</param>
/// <param name="Code">The 6-digit TOTP code to confirm the action.</param>
public record Disable2FaCommand(Guid UserId, string Code) : IRequest;
