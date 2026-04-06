namespace FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Domain.Enums;
using MediatR;

/// <summary>Command to record a new financial transaction against a family account.</summary>
/// <param name="FamilyId">The owning family's identifier.</param>
/// <param name="AccountId">The account to record the transaction against.</param>
/// <param name="UserId">The user recording the transaction.</param>
/// <param name="CategoryId">The category for classification.</param>
/// <param name="Type">Income or Expense.</param>
/// <param name="Amount">The transaction amount. Must be positive.</param>
/// <param name="Currency">ISO 4217 currency code (e.g., "CLP").</param>
/// <param name="Description">A short description of the transaction.</param>
/// <param name="TransactionDate">The date on which the transaction occurred.</param>
public record CreateTransactionCommand(
    Guid FamilyId,
    Guid AccountId,
    Guid UserId,
    Guid CategoryId,
    TransactionType Type,
    decimal Amount,
    string Currency,
    string Description,
    DateOnly TransactionDate
) : IRequest<TransactionDto>;

/// <summary>Projection of a <see cref="FinanceApp.Domain.Entities.Transaction"/> for API responses.</summary>
/// <param name="Id">The transaction's unique identifier.</param>
/// <param name="FamilyId">The owning family's identifier.</param>
/// <param name="AccountId">The account this transaction is recorded against.</param>
/// <param name="UserId">The user who created the transaction.</param>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="CategoryName">The category display name.</param>
/// <param name="Type">Transaction type as a string ("Income" or "Expense").</param>
/// <param name="Amount">The transaction amount.</param>
/// <param name="Currency">ISO 4217 currency code.</param>
/// <param name="Description">Description of the transaction.</param>
/// <param name="TransactionDate">The date of the transaction.</param>
/// <param name="CreatedAt">The UTC timestamp when the record was created.</param>
public record TransactionDto(
    Guid Id, Guid FamilyId, Guid AccountId, Guid UserId, Guid CategoryId,
    string CategoryName, string Type, decimal Amount, string Currency, string Description,
    DateOnly TransactionDate, DateTime CreatedAt);
