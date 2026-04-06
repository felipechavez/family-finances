namespace FinanceApp.Application.Features.Auth.Register;
using MediatR;

/// <summary>Command to register a new user account.</summary>
/// <param name="Name">The user's display name.</param>
/// <param name="Email">The user's email address. Must be unique.</param>
/// <param name="Password">The desired plain-text password (will be hashed).</param>
public record RegisterCommand(string Name, string Email, string Password) : IRequest<RegisterResult>;

/// <summary>Result returned after a successful registration.</summary>
/// <param name="UserId">The newly created user's identifier.</param>
/// <param name="Email">The registered email address.</param>
public record RegisterResult(Guid UserId, string Email);
