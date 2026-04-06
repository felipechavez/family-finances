namespace FinanceApp.Application.Features.Auth.Login;
using FinanceApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handles <see cref="LoginCommand"/>: validates credentials and issues a JWT access token.
/// </summary>
public class LoginHandler(IAppDbContext db, IPasswordHasher hasher, IJwtService jwt, ILogger<LoginHandler> logger)
    : IRequestHandler<LoginCommand, LoginResult>
{
    /// <inheritdoc/>
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var membership = await db.FamilyMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == user.Id, cancellationToken);

        var token = jwt.GenerateToken(user, membership?.FamilyId, membership?.Role.ToString());

        logger.LogInformation("User {UserId} authenticated successfully", user.Id);

        return new LoginResult(token, user.Id, user.Name, user.Email, membership?.FamilyId);
    }
}
