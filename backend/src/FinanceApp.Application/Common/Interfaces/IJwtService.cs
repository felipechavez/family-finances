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
    string GenerateToken(Users user, Guid? familyId = null, string? role = null);

    /// <summary>
    /// Generates a short-lived (5-minute) challenge token used during the 2FA verification step.
    /// The token carries only the user ID and a <c>scope=2fa_challenge</c> claim.
    /// </summary>
    string GenerateChallengeToken(Guid userId);

    /// <summary>
    /// Validates a 2FA challenge token. Returns the user ID if the token is valid and unexpired;
    /// returns <c>null</c> if invalid, expired, or not a challenge token.
    /// </summary>
    Guid? ValidateChallengeToken(string token);
}
