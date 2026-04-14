namespace FinanceApp.Application.Features.Auth.Disable2Fa;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="Disable2FaCommand"/>: validates the current TOTP code, then disables 2FA
/// and clears the stored secret.
/// </summary>
public class Disable2FaHandler(Client supabase, ITotpService totp, ILogger<Disable2FaHandler> logger)
    : IRequestHandler<Disable2FaCommand>
{
    public async Task Handle(Disable2FaCommand request, CancellationToken cancellationToken)
    {
        var response = await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Get();

        var user = response.Model
            ?? throw new AppException(LocalizationKeys.Auth_InvalidCredentials, 401);

        if (!user.TwoFactorEnabled)
            throw new AppException(LocalizationKeys.Auth_TwoFactorNotEnabled, 409);

        if (!totp.VerifyCode(user.TotpSecret!, request.Code))
            throw new AppException(LocalizationKeys.Auth_InvalidTotpCode, 400);

        user.TwoFactorEnabled = false;
        user.TotpSecret = null;
        await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Update(user);

        logger.LogInformation("2FA disabled for user {UserId}", request.UserId);
    }
}
