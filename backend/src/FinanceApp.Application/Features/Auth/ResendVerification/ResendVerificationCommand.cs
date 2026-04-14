namespace FinanceApp.Application.Features.Auth.ResendVerification;
using MediatR;

/// <summary>Command to resend the email verification link to an unverified user.</summary>
/// <param name="Email">The user's registered email address.</param>
public record ResendVerificationCommand(string Email) : IRequest;
