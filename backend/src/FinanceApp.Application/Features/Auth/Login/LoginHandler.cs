namespace FinanceApp.Application.Features.Auth.Login;
using FinanceApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class LoginHandler(IAppDbContext db, IPasswordHasher hasher, IJwtService jwt)
    : IRequestHandler<LoginCommand, LoginResult>
{
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
        return new LoginResult(token, user.Id, user.Name, user.Email, membership?.FamilyId);
    }
}
