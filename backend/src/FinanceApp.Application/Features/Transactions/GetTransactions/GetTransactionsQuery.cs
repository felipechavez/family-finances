namespace FinanceApp.Application.Features.Transactions.GetTransactions;
using FinanceApp.Application.Features.Transactions.CreateTransaction;
using MediatR;

public record GetTransactionsQuery(Guid FamilyId, int? Month = null, int? Year = null)
    : IRequest<IReadOnlyList<TransactionDto>>;
