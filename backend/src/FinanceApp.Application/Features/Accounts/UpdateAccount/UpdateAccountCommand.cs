namespace FinanceApp.Application.Features.Accounts.UpdateAccount;
using MediatR;

/// <summary>Command to rename an account and set its current balance.</summary>
/// <param name="AccountId">The account to update.</param>
/// <param name="FamilyId">The caller's family — used to enforce ownership.</param>
/// <param name="Name">New display name.</param>
/// <param name="Balance">New current balance.</param>
public record UpdateAccountCommand(Guid AccountId, Guid FamilyId, string Name, decimal Balance)
    : IRequest;
