namespace FinanceApp.Application.Features.Transactions.GetTransactions;
using FinanceApp.Application.Features.Transactions.CreateTransaction;
using MediatR;

/// <summary>Query to retrieve transactions for a family, optionally filtered by month and year.</summary>
/// <param name="FamilyId">The family whose transactions to retrieve.</param>
/// <param name="Month">Optional month filter (1–12).</param>
/// <param name="Year">Optional year filter (e.g., 2025).</param>
public record GetTransactionsQuery(Guid FamilyId, int? Month = null, int? Year = null)
    : IRequest<IReadOnlyList<TransactionDto>>;
