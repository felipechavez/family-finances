namespace FinanceApp.Infrastructure.Services;
using FinanceApp.Application.Common.Interfaces;
using OtpNet;

/// <summary>
/// TOTP implementation using the <c>Otp.NET</c> library (RFC 6238).
/// </summary>
public sealed class TotpService : ITotpService
{
    /// <inheritdoc/>
    public string GenerateSecret()
    {
        var secretBytes = KeyGeneration.GenerateRandomKey(20); // 160-bit secret
        return Base32Encoding.ToString(secretBytes);
    }

    /// <inheritdoc/>
    public string BuildProvisioningUri(string secret, string userEmail, string issuer = "FinanzasApp")
    {
        var encodedEmail = Uri.EscapeDataString(userEmail);
        var encodedIssuer = Uri.EscapeDataString(issuer);
        return $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={secret}&issuer={encodedIssuer}&algorithm=SHA1&digits=6&period=30";
    }

    /// <inheritdoc/>
    public bool VerifyCode(string secret, string code)
    {
        try
        {
            var secretBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(secretBytes);
            return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
        }
        catch
        {
            return false;
        }
    }
}
