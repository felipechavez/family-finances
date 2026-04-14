namespace FinanceApp.Application.Features.Accounts.UpdateAccount;

using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="UpdateAccountCommand"/>: renames an account and adjusts its balance.
/// Enforces family ownership and prevents duplicate names within the same family.
/// </summary>
public class UpdateAccountHandler(Client supabase, ILogger<UpdateAccountHandler> logger)
    : IRequestHandler<UpdateAccountCommand>
{
    public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var accountResponse = await supabase.From<Account>()
            .Where(a => a.Id == request.AccountId && a.FamilyId == request.FamilyId)
            .Get();

        var account = accountResponse.Model
            ?? throw new AppException(LocalizationKeys.Account_NotFound, 404);

        // Check name collision only when the name actually changed.
        if (!string.Equals(account.Name, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var duplicate = await supabase.From<Account>()
                .Where(a => a.FamilyId == request.FamilyId && a.Name == request.Name)
                .Get();

            if (duplicate.Model is not null)
                throw new AppException(LocalizationKeys.Account_DuplicateName, 409);
        }

        account.Name    = request.Name;
        account.Balance = request.Balance;

        await supabase.From<Account>()
            .Where(a => a.Id == request.AccountId && a.FamilyId == request.FamilyId)
            .Update(account);

        logger.LogInformation("Account {AccountId} updated for family {FamilyId}",
            request.AccountId, request.FamilyId);
    }
}
