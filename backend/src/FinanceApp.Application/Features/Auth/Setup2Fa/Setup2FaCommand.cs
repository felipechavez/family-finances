namespace FinanceApp.Application.Features.Auth.Setup2Fa;
using MediatR;

/// <summary>Command to begin 2FA setup: generates a TOTP secret and returns the provisioning URI.</summary>
/// <param name="UserId">The authenticated user requesting 2FA setup.</param>
public record Setup2FaCommand(Guid UserId) : IRequest<Setup2FaResult>;

/// <summary>Result of a 2FA setup request.</summary>
/// <param name="Secret">The base32-encoded TOTP secret (shown to user as backup).</param>
/// <param name="ProvisioningUri">The <c>otpauth://</c> URI for QR-code scanning.</param>
public record Setup2FaResult(string Secret, string ProvisioningUri);
