namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using Supabase.Postgrest.Attributes;

/// <summary>
/// Represents an application user who can belong to one or more families.
/// </summary>
[Table("user")]
public class User : Entity
{
    /// <summary>Gets the user's display name.</summary>

    [Column("name")]
    public string Name { get; private set; } = default!;

    /// <summary>Gets the user's unique email address used for authentication.</summary>
    [Column("email")]
    public string Email { get; private set; } = default!;

    /// <summary>Gets the BCrypt hash of the user's password.</summary>
    [Column("passwordHash")]
    public string PasswordHash { get; private set; } = default!;

    private User() { }

    /// <summary>
    /// Creates a new <see cref="User"/> with the provided credentials.
    /// </summary>
    /// <param name="name">The user's display name. Must not be null or whitespace.</param>
    /// <param name="email">The user's email address. Must not be null or whitespace.</param>
    /// <param name="passwordHash">The pre-hashed password. Must not be null or whitespace.</param>
    /// <returns>A new <see cref="User"/> instance.</returns>
    public static User Create(string name, string email, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        return new User { Name = name, Email = email, PasswordHash = passwordHash };
    }
}
