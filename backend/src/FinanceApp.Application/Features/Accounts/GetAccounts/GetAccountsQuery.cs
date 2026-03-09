namespace FinanceApp.Application.Features.Accounts.GetAccounts;
using MediatR;

public record GetAccountsQuery(Guid FamilyId) : IRequest<IReadOnlyList<AccountDto>>;
public record AccountDto(Guid Id, Guid FamilyId, string Name, string Type, decimal Balance, DateTime CreatedAt);
