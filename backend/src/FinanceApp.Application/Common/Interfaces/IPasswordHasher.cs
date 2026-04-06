namespace FinanceApp.Application.Common.Interfaces;

/// <summary>
/// Provides one-way password hashing and verification using a secure algorithm (BCrypt).
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password using BCrypt.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>A BCrypt hash string safe for storage.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifies a plain-text password against a stored BCrypt hash.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="hash">The stored BCrypt hash.</param>
    /// <returns><c>true</c> if the password matches the hash; otherwise <c>false</c>.</returns>
    bool Verify(string password, string hash);
}
