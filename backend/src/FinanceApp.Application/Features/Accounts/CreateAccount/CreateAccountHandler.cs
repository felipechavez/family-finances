namespace FinanceApp.Application.Features.Accounts.CreateAccount;

using FinanceApp.Application.Common;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="CreateAccountCommand"/>: creates and persists a new financial account.
/// </summary>
public class CreateAccountHandler(Client supabase, ILogger<CreateAccountHandler> logger)
    : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var existing = await supabase.From<Account>()
            .Where(a => a.FamilyId == request.FamilyId && a.Name == request.Name)
            .Get();

        if (existing.Model is not null)
            throw new AppException(LocalizationKeys.Account_DuplicateName, 409);

        var account = Account.Create(request.FamilyId, request.Name, request.Type, request.InitialBalance);

        await supabase.From<Account>().Insert(account);

        logger.LogInformation("Account {AccountId} created for family {FamilyId}",
            account.Id, request.FamilyId);

        return account.Id;
    }
}
