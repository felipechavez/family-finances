namespace FinanceApp.Application.Features.Auth.ResendVerification;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="ResendVerificationCommand"/>: generates a new verification token and resends the email.
/// Silently ignores requests for unknown or already-verified addresses to prevent user enumeration.
/// </summary>
public class ResendVerificationHandler(Client supabase, IEmailService email, ILogger<ResendVerificationHandler> logger)
    : IRequestHandler<ResendVerificationCommand>
{
    public async Task Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
    {
        var response = await supabase.From<Users>()
            .Where(u => u.Email == request.Email)
            .Get();

        var user = response.Model;

        // Silently return when email is unknown or already verified (prevents enumeration).
        if (user is null || user.EmailVerified) return;

        user.VerificationToken = Guid.NewGuid().ToString("N");
        user.VerificationTokenExp = DateTime.UtcNow.AddHours(24);

        await supabase.From<Users>()
            .Where(u => u.Id == user.Id)
            .Update(user);

        await email.SendVerificationEmailAsync(user.Email, user.Name, user.VerificationToken, cancellationToken);

        logger.LogInformation("Verification email resent to {Email}", request.Email);
    }
}
