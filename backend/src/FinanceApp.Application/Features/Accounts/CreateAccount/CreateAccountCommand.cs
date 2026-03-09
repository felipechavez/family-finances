namespace FinanceApp.Application.Features.Accounts.CreateAccount;
using FinanceApp.Domain.Enums;
using MediatR;

public record CreateAccountCommand(Guid FamilyId, string Name, AccountType Type, decimal InitialBalance = 0)
    : IRequest<Guid>;
