namespace FinanceApp.Application.Features.Auth.Login;
using MediatR;

/// <summary>Command to authenticate a user and return a JWT token.</summary>
/// <param name="Email">The user's registered email address.</param>
/// <param name="Password">The user's plain-text password.</param>
public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

/// <summary>Result returned after a successful login.</summary>
/// <param name="Token">The signed JWT access token. Empty when <paramref name="RequiresTwoFactor"/> is true.</param>
/// <param name="UserId">The authenticated user's identifier.</param>
/// <param name="Name">The authenticated user's display name.</param>
/// <param name="Email">The authenticated user's email address.</param>
/// <param name="FamilyId">The user's active family identifier, or <c>null</c> if not yet in a family.</param>
/// <param name="RequiresTwoFactor">True when the account has 2FA enabled and the client must complete verification.</param>
/// <param name="ChallengeToken">Short-lived token used to complete the 2FA step. Present only when <paramref name="RequiresTwoFactor"/> is true.</param>
public record LoginResult(
    string Token,
    Guid UserId,
    string Name,
    string Email,
    Guid? FamilyId,
    bool RequiresTwoFactor = false,
    string? ChallengeToken = null);
