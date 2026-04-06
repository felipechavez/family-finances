namespace FinanceApp.Application.Features.Accounts.CreateAccount;
using FinanceApp.Domain.Enums;
using MediatR;

/// <summary>Command to create a new financial account for a family.</summary>
/// <param name="FamilyId">The owning family's identifier.</param>
/// <param name="Name">The display name of the account.</param>
/// <param name="Type">The account type (Cash, Bank, Savings, CreditCard).</param>
/// <param name="InitialBalance">The starting balance. Defaults to zero.</param>
public record CreateAccountCommand(Guid FamilyId, string Name, AccountType Type, decimal InitialBalance = 0)
    : IRequest<Guid>;
