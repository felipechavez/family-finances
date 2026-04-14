namespace FinanceApp.Application.Features.Auth.Register;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supabase;

/// <summary>
/// Handles <see cref="RegisterCommand"/>: creates a new user account with a hashed password
/// and optionally sends an email verification link.
/// When <see cref="EmailVerificationOptions.Enabled"/> is <c>false</c> the account is
/// pre-verified so that local development works without a real email service.
/// </summary>
public class RegisterHandler(
    Client supabase,
    IPasswordHasher hasher,
    IEmailService email,
    IOptions<EmailVerificationOptions> emailVerificationOptions,
    ILogger<RegisterHandler> logger)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await supabase.From<Users>()
            .Where(u => u.Email == request.Email)
            .Get();

        if (existing.Model is not null)
            throw new AppException(LocalizationKeys.Auth_EmailAlreadyRegistered, 409);

        var hash = hasher.Hash(request.Password);
        var user = Users.Create(request.Name, request.Email, hash);

        bool verificationEnabled = emailVerificationOptions.Value.Enabled;

        if (verificationEnabled)
        {
            // Generate a 32-character hex verification token valid for 24 hours.
            user.VerificationToken = Guid.NewGuid().ToString("N");
            user.VerificationTokenExp = DateTime.UtcNow.AddHours(24);
        }
        else
        {
            // Dev mode: mark as verified immediately.
            user.EmailVerified = true;
        }

        await supabase.From<Users>().Insert(user);

        logger.LogInformation("New user registered: {UserId} ({Email}), emailVerified={Verified}",
            user.Id, user.Email, user.EmailVerified);

        if (verificationEnabled)
            await email.SendVerificationEmailAsync(user.Email, user.Name, user.VerificationToken!, cancellationToken);

        return new RegisterResult(user.Id, user.Email);
    }
}
