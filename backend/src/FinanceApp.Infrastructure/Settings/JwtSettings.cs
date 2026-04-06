namespace FinanceApp.Infrastructure.Settings;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Strongly-typed configuration for JWT token generation and validation.
/// Bind this via <c>appsettings.json</c> under the <c>Jwt</c> section.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>Gets or sets the HMAC-SHA256 signing secret. Must be at least 32 characters.</summary>
    [Required, MinLength(32)]
    public string Secret { get; set; } = default!;

    /// <summary>Gets or sets the token issuer claim (iss).</summary>
    [Required]
    public string Issuer { get; set; } = default!;

    /// <summary>Gets or sets the token audience claim (aud).</summary>
    [Required]
    public string Audience { get; set; } = default!;

    /// <summary>Gets or sets the number of days before a token expires. Defaults to 7.</summary>
    public int ExpirationDays { get; set; } = 7;
}
