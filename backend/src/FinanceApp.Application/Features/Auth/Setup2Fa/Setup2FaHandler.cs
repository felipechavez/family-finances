namespace FinanceApp.Application.Features.Auth.Setup2Fa;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="Setup2FaCommand"/>: generates a TOTP secret and stores it (unconfirmed).
/// The secret is activated only after the user confirms with a valid code via <c>POST /auth/confirm-2fa</c>.
/// </summary>
public class Setup2FaHandler(Client supabase, ITotpService totp, ILogger<Setup2FaHandler> logger)
    : IRequestHandler<Setup2FaCommand, Setup2FaResult>
{
    public async Task<Setup2FaResult> Handle(Setup2FaCommand request, CancellationToken cancellationToken)
    {
        var response = await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Get();

        var user = response.Model
            ?? throw new AppException(LocalizationKeys.Auth_InvalidCredentials, 401);

        if (user.TwoFactorEnabled)
            throw new AppException(LocalizationKeys.Auth_TwoFactorAlreadyEnabled, 409);

        var secret = totp.GenerateSecret();
        var uri = totp.BuildProvisioningUri(secret, user.Email);

        // Store the secret but do NOT enable 2FA yet — confirmation is required.
        user.TotpSecret = secret;
        await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Update(user);

        logger.LogInformation("2FA setup initiated for user {UserId}", request.UserId);

        return new Setup2FaResult(secret, uri);
    }
}
