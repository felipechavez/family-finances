namespace FinanceApp.Application.Features.Auth.ChangeEmail.ConfirmEmailChange;
using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Supabase;

public class ConfirmEmailChangeHandler(Client supabase) : IRequestHandler<ConfirmEmailChangeCommand>
{
    public async Task Handle(ConfirmEmailChangeCommand request, CancellationToken cancellationToken)
    {
        var result = await supabase.From<Users>()
            .Where(u => u.EmailChangeToken == request.Token)
            .Get();

        var user = result.Model ?? throw new AppException(LocalizationKeys.Auth_InvalidEmailChangeToken, 400);

        if (user.EmailChangeTokenExp is null || user.EmailChangeTokenExp < DateTime.UtcNow)
            throw new AppException(LocalizationKeys.Auth_EmailChangeTokenExpired, 400);

        var newEmail = user.PendingEmail!;

        await supabase.From<Users>()
            .Where(u => u.Id == user.Id)
            .Set(u => u.Email,              newEmail)
            .Set(u => u.PendingEmail,       (string?)null)
            .Set(u => u.EmailChangeToken,   (string?)null)
            .Set(u => u.EmailChangeTokenExp,(DateTime?)null)
            .Update();
    }
}
