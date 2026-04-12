namespace FinanceApp.Application.Features.Auth.Register;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="RegisterCommand"/>: creates a new user account with a hashed password.
/// </summary>
public class RegisterHandler(Client supabase, IPasswordHasher hasher, ILogger<RegisterHandler> logger)
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await supabase.From<Users>()
            .Where(u => u.Email == request.Email)
            .Get();

        if (existing.Model is not null)
            throw new AppException(LocalizationKeys.Auth_EmailAlreadyRegistered, 409);

        var hash = hasher.Hash(request.Password);
        var user = Users.Create(request.Name, request.Email, hash);

        await supabase.From<Users>().Insert(user);

        logger.LogInformation("New user registered: {UserId} ({Email})", user.Id, user.Email);

        return new RegisterResult(user.Id, user.Email);
    }
}
