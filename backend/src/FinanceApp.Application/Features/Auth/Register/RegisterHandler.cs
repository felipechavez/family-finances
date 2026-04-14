namespace FinanceApp.Application.Features.Auth.Register;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="RegisterCommand"/>: creates a new user account with a hashed password
/// and sends an email verification link.
/// </summary>
public class RegisterHandler(
    Client supabase,
    IPasswordHasher hasher,
    IEmailService email,
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

        // Generate a 32-character hex verification token valid for 24 hours.
        user.VerificationToken = Guid.NewGuid().ToString("N");
        user.VerificationTokenExp = DateTime.UtcNow.AddHours(24);

        await supabase.From<Users>().Insert(user);

        logger.LogInformation("New user registered: {UserId} ({Email})", user.Id, user.Email);

        // Fire-and-forget — email service already catches and logs its own exceptions.
        await email.SendVerificationEmailAsync(user.Email, user.Name, user.VerificationToken, cancellationToken);

        return new RegisterResult(user.Id, user.Email);
    }
}
