namespace FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;

/// <summary>
/// Generates signed JWT access tokens for authenticated users.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Creates a signed JWT token for the given user, optionally embedding family context and role.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <param name="familyId">The user's active family, if any.</param>
    /// <param name="role">The user's role within the family, if any.</param>
    /// <returns>A signed JWT token string.</returns>
    string GenerateToken(User user, Guid? familyId = null, string? role = null);
}
