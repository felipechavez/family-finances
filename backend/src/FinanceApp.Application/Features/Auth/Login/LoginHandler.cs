namespace FinanceApp.Application.Features.Auth.Login;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="LoginCommand"/>: validates credentials and issues a JWT access token.
/// </summary>
public class LoginHandler(Client supabase, IPasswordHasher hasher, IJwtService jwt, ILogger<LoginHandler> logger)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userResponse = await supabase.From<Users>()
            .Where(u => u.Email == request.Email)
            .Get();

        var user = userResponse.Model
            ?? throw new AppException(LocalizationKeys.Auth_InvalidCredentials, 401);

        if (!hasher.Verify(request.Password, user.PasswordHash))
            throw new AppException(LocalizationKeys.Auth_InvalidCredentials, 401);

        var membershipResponse = await supabase.From<FamilyMember>()
            .Where(m => m.UserId == user.Id)
            .Get();

        var membership = membershipResponse.Model;
        var token = jwt.GenerateToken(user, membership?.FamilyId, membership?.Role.ToString());

        logger.LogInformation("User {UserId} authenticated successfully", user.Id);

        return new LoginResult(token, user.Id, user.Name, user.Email, membership?.FamilyId);
    }
}
