namespace FinanceApp.Application.Common.Interfaces;

/// <summary>
/// Generates and verifies TOTP codes (RFC 6238) for two-factor authentication.
/// </summary>
public interface ITotpService
{
    /// <summary>
    /// Generates a cryptographically secure base32-encoded TOTP secret.
    /// </summary>
    string GenerateSecret();

    /// <summary>
    /// Builds the <c>otpauth://</c> URI for QR-code provisioning (e.g., Google Authenticator).
    /// </summary>
    string BuildProvisioningUri(string secret, string userEmail, string issuer = "DomusPay");

    /// <summary>
    /// Verifies a 6-digit TOTP code against the given secret.
    /// Allows a ±1-step window (±30 s) to account for clock skew.
    /// </summary>
    bool VerifyCode(string secret, string code);
}
