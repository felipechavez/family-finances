namespace FinanceApp.Application.Features.Auth.ChangePassword;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Supabase;

public class ChangePasswordHandler(Client supabase, IPasswordHasher hasher) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Get();

        var user = result.Model ?? throw new AppException(LocalizationKeys.Auth_UserNotFound, 404);

        if (!hasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new AppException(LocalizationKeys.Auth_InvalidCurrentPassword, 400);

        if (hasher.Verify(request.NewPassword, user.PasswordHash))
            throw new AppException(LocalizationKeys.Auth_PasswordSameAsCurrent, 400);

        var newHash = hasher.Hash(request.NewPassword);

        await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Set(u => u.PasswordHash, newHash)
            .Update();
    }
}
