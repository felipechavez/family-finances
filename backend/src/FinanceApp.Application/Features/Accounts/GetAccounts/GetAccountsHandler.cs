namespace FinanceApp.Application.Features.Accounts.GetAccounts;
using FinanceApp.Domain.Entities;
using MediatR;
using Supabase;

/// <summary>
/// Handles <see cref="GetAccountsQuery"/>: retrieves all accounts belonging to a family.
/// </summary>
public class GetAccountsHandler(Client supabase) : IRequestHandler<GetAccountsQuery, IReadOnlyList<AccountDto>>
{
    public async Task<IReadOnlyList<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var response = await supabase.From<Account>()
            .Where(a => a.FamilyId == request.FamilyId)
            .Get();

        return response.Models?
            .Select(a => new AccountDto(a.Id, a.FamilyId, a.Name, a.Type.ToString(), a.Balance, a.CreatedAt))
            .ToList()
            ?? new List<AccountDto>();
    }
}
