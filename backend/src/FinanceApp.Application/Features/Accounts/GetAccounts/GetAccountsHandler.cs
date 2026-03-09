namespace FinanceApp.Application.Features.Accounts.GetAccounts;
using FinanceApp.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetAccountsHandler(IAppDbContext db) : IRequestHandler<GetAccountsQuery, IReadOnlyList<AccountDto>>
{
    public async Task<IReadOnlyList<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        => await db.Accounts
            .AsNoTracking()
            .Where(a => a.FamilyId == request.FamilyId)
            .Select(a => new AccountDto(a.Id, a.FamilyId, a.Name, a.Type.ToString(), a.Balance, a.CreatedAt))
            .ToListAsync(cancellationToken);
}
