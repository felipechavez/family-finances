using FinanceApp.Domain.Common;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Serialization;

namespace FinanceApp.Domain.Entities;

/// <summary>
/// Represents an application user who can belong to one or more families.
/// </summary>
[Table("users")]
public class Users : Entity
{
    /// <summary>Gets the user's display name.</summary>
    [Column("name")]
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>Gets the user's unique email address used for authentication.</summary>
    [Column("email")]
    [JsonInclude]
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    /// <summary>Gets the BCrypt hash of the user's password.</summary>
    [Column("password_hash")]
    [JsonInclude]
    [JsonPropertyName("password_hash")]
    public string PasswordHash { get; set; } = default!;

    public Users() { }

    /// <summary>Whether the user has verified their email address.</summary>
    [Column("email_verified")]
    [JsonInclude]
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; } = false;

    /// <summary>One-time token sent to the user's email for verification.</summary>
    [Column("verification_token")]
    [JsonInclude]
    [JsonPropertyName("verification_token")]
    public string? VerificationToken { get; set; }

    /// <summary>UTC expiration time for the verification token.</summary>
    [Column("verification_token_exp")]
    [JsonInclude]
    [JsonPropertyName("verification_token_exp")]
    public DateTime? VerificationTokenExp { get; set; }

    /// <summary>Whether TOTP-based two-factor authentication is enabled.</summary>
    [Column("two_factor_enabled")]
    [JsonInclude]
    [JsonPropertyName("two_factor_enabled")]
    public bool TwoFactorEnabled { get; set; } = false;

    /// <summary>Base32-encoded TOTP secret. Null when 2FA is not set up.</summary>
    [Column("totp_secret")]
    [JsonInclude]
    [JsonPropertyName("totp_secret")]
    public string? TotpSecret { get; set; }

    /// <summary>Whether this user receives a daily email summary of family finances.</summary>
    [Column("daily_summary_enabled")]
    [JsonInclude]
    [JsonPropertyName("daily_summary_enabled")]
    public bool DailySummaryEnabled { get; set; } = true;

    /// <summary>New email address awaiting confirmation during an email-change flow.</summary>
    [Column("pending_email")]
    [JsonInclude]
    [JsonPropertyName("pending_email")]
    public string? PendingEmail { get; set; }

    /// <summary>One-time token sent to the new address to confirm the email change.</summary>
    [Column("email_change_token")]
    [JsonInclude]
    [JsonPropertyName("email_change_token")]
    public string? EmailChangeToken { get; set; }

    /// <summary>UTC expiration time for the email-change token.</summary>
    [Column("email_change_token_exp")]
    [JsonInclude]
    [JsonPropertyName("email_change_token_exp")]
    public DateTime? EmailChangeTokenExp { get; set; }

    /// <summary>
    /// Creates a new <see cref="Users"/> with the provided credentials.
    /// </summary>
    /// <param name="name">The user's display name. Must not be null or whitespace.</param>
    /// <param name="email">The user's email address. Must not be null or whitespace.</param>
    /// <param name="passwordHash">The pre-hashed password. Must not be null or whitespace.</param>
    /// <returns>A new <see cref="Users"/> instance.</returns>
    public static Users Create(string name, string email, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        return new Users { Name = name, Email = email, PasswordHash = passwordHash };
    }
}