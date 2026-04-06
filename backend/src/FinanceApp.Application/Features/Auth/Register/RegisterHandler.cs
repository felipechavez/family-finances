namespace FinanceApp.Application.Features.Auth.Register;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handles <see cref="RegisterCommand"/>: creates a new user account with a hashed password.
/// </summary>
public class RegisterHandler(IAppDbContext db, IPasswordHasher hasher, ILogger<RegisterHandler> logger)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    /// <inheritdoc/>
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (exists) throw new InvalidOperationException("Email already registered.");

        var hash = hasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, hash);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("New user registered: {UserId} ({Email})", user.Id, user.Email);

        return new RegisterResult(user.Id, user.Email);
    }
}
