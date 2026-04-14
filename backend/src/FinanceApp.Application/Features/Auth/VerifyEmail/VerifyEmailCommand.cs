namespace FinanceApp.Application.Features.Auth.VerifyEmail;
using MediatR;

/// <summary>Command to verify a user's email address using the one-time token sent during registration.</summary>
/// <param name="Token">The 32-character hex verification token from the email link.</param>
public record VerifyEmailCommand(string Token) : IRequest;
