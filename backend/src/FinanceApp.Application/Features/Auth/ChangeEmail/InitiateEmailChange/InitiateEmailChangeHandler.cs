namespace FinanceApp.Application.Features.Auth.ChangeEmail.InitiateEmailChange;
using System.Security.Cryptography;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Supabase;

public class InitiateEmailChangeHandler(Client supabase, IEmailService emailService) : IRequestHandler<InitiateEmailChangeCommand>
{
    public async Task Handle(InitiateEmailChangeCommand request, CancellationToken cancellationToken)
    {
        // Ensure the new email is not already taken
        var existing = await supabase.From<Users>()
            .Where(u => u.Email == request.NewEmail)
            .Get();

        if (existing.Model is not null)
            throw new AppException(LocalizationKeys.Auth_EmailAlreadyInUse, 409);

        // Load the current user
        var result = await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Get();

        var user = result.Model ?? throw new AppException(LocalizationKeys.Auth_UserNotFound, 404);

        // Generate a secure one-time token (128-bit hex = 32 chars)
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
        var exp   = DateTime.UtcNow.AddHours(24);

        await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Set(u => u.PendingEmail,         request.NewEmail)
            .Set(u => u.EmailChangeToken,     token)
            .Set(u => u.EmailChangeTokenExp,  exp)
            .Update();

        // Send confirmation link to the new address and a security notice to the current one
        await emailService.SendEmailChangeConfirmationAsync(request.NewEmail, user.Name, token, cancellationToken);
        await emailService.SendEmailChangeNotificationAsync(user.Email, user.Name, cancellationToken);
    }
}
