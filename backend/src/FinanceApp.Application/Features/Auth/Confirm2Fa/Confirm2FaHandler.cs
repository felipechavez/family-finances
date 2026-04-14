namespace FinanceApp.Application.Features.Auth.Confirm2Fa;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="Confirm2FaCommand"/>: verifies the first TOTP code and enables 2FA on the account.
/// </summary>
public class Confirm2FaHandler(Client supabase, ITotpService totp, ILogger<Confirm2FaHandler> logger)
    : IRequestHandler<Confirm2FaCommand>
{
    public async Task Handle(Confirm2FaCommand request, CancellationToken cancellationToken)
    {
        var response = await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Get();

        var user = response.Model
            ?? throw new AppException(LocalizationKeys.Auth_InvalidCredentials, 401);

        if (user.TwoFactorEnabled)
            throw new AppException(LocalizationKeys.Auth_TwoFactorAlreadyEnabled, 409);

        if (string.IsNullOrEmpty(user.TotpSecret))
            throw new AppException(LocalizationKeys.Auth_InvalidToken, 400);

        if (!totp.VerifyCode(user.TotpSecret, request.Code))
            throw new AppException(LocalizationKeys.Auth_InvalidTotpCode, 400);

        user.TwoFactorEnabled = true;
        await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Update(user);

        logger.LogInformation("2FA enabled for user {UserId}", request.UserId);
    }
}
