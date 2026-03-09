namespace FinanceApp.Application.Features.Auth.Register;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class RegisterHandler(IAppDbContext db, IPasswordHasher hasher) : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (exists) throw new InvalidOperationException("Email already registered.");

        var hash = hasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, hash);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return new RegisterResult(user.Id, user.Email);
    }
}
