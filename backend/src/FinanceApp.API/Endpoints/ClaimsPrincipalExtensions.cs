namespace FinanceApp.API.Endpoints;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/// <summary>
/// Extension methods for extracting typed claim values from a <see cref="ClaimsPrincipal"/>.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns the family identifier embedded in the JWT, or throws if the claim is absent.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user has no family claim.</exception>
    internal static Guid GetFamilyId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue("family_id")
            ?? throw new UnauthorizedAccessException("No family associated with this user.");
        return Guid.Parse(claim);
    }

    /// <summary>
    /// Returns the user identifier from the JWT subject claim, or throws if absent.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user identity claim is missing.</exception>
    internal static Guid GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new UnauthorizedAccessException("User identity not found.");
        return Guid.Parse(claim);
    }
}
