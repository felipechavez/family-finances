namespace FinanceApp.Application.Features.Auth.VerifyEmail;
using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="VerifyEmailCommand"/>: validates the token, marks the user as verified,
/// and clears the token fields.
/// </summary>
public class VerifyEmailHandler(Client supabase, ILogger<VerifyEmailHandler> logger)
    : IRequestHandler<VerifyEmailCommand>
{
    public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var response = await supabase.From<Users>()
            .Where(u => u.VerificationToken == request.Token)
            .Get();

        var user = response.Model
            ?? throw new AppException(LocalizationKeys.Auth_InvalidToken, 400);

        if (user.EmailVerified)
            throw new AppException(LocalizationKeys.Auth_AlreadyVerified, 409);

        if (user.VerificationTokenExp is null || user.VerificationTokenExp < DateTime.UtcNow)
            throw new AppException(LocalizationKeys.Auth_TokenExpired, 400);

        user.EmailVerified = true;
        user.VerificationToken = null;
        user.VerificationTokenExp = null;

        await supabase.From<Users>()
            .Where(u => u.Id == user.Id)
            .Update(user);

        logger.LogInformation("Email verified for user {UserId}", user.Id);
    }
}
