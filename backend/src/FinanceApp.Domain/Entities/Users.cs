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