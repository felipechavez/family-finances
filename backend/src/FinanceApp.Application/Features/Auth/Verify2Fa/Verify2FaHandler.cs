namespace FinanceApp.Application.Features.Auth.Verify2Fa;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Application.Features.Auth.Login;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="Verify2FaCommand"/>: validates the 2FA challenge token and the TOTP code,
/// then issues the full JWT access token.
/// </summary>
public class Verify2FaHandler(
    Client supabase,
    IJwtService jwt,
    ITotpService totp,
    ILogger<Verify2FaHandler> logger)
    : IRequestHandler<Verify2FaCommand, LoginResult>
{
    public async Task<LoginResult> Handle(Verify2FaCommand request, CancellationToken cancellationToken)
    {
        var userId = jwt.ValidateChallengeToken(request.ChallengeToken)
            ?? throw new AppException(LocalizationKeys.Auth_InvalidToken, 401);

        var userResponse = await supabase.From<Users>()
            .Where(u => u.Id == userId)
            .Get();

        var user = userResponse.Model
            ?? throw new AppException(LocalizationKeys.Auth_InvalidToken, 401);

        if (!user.TwoFactorEnabled || string.IsNullOrEmpty(user.TotpSecret))
            throw new AppException(LocalizationKeys.Auth_InvalidToken, 401);

        if (!totp.VerifyCode(user.TotpSecret, request.Code))
            throw new AppException(LocalizationKeys.Auth_InvalidTotpCode, 401);

        var memberResponse = await supabase.From<FamilyMember>()
            .Where(m => m.UserId == userId)
            .Get();

        var membership = memberResponse.Model;
        var token = jwt.GenerateToken(user, membership?.FamilyId, membership?.Role.ToString());

        logger.LogInformation("User {UserId} completed 2FA verification", userId);

        return new LoginResult(token, user.Id, user.Name, user.Email, membership?.FamilyId);
    }
}
