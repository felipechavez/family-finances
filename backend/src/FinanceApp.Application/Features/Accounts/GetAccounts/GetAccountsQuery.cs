namespace FinanceApp.Application.Features.Accounts.GetAccounts;
using MediatR;

/// <summary>Query to retrieve all accounts belonging to a family.</summary>
/// <param name="FamilyId">The family whose accounts to retrieve.</param>
public record GetAccountsQuery(Guid FamilyId) : IRequest<IReadOnlyList<AccountDto>>;

/// <summary>Projection of an <see cref="FinanceApp.Domain.Entities.Account"/> for API responses.</summary>
/// <param name="Id">The account's unique identifier.</param>
/// <param name="FamilyId">The owning family's identifier.</param>
/// <param name="Name">The account's display name.</param>
/// <param name="Type">The account type as a string (e.g., "Bank", "Cash").</param>
/// <param name="Balance">The current balance.</param>
/// <param name="CreatedAt">The UTC timestamp when the account was created.</param>
public record AccountDto(Guid Id, Guid FamilyId, string Name, string Type, decimal Balance, DateTime CreatedAt);
