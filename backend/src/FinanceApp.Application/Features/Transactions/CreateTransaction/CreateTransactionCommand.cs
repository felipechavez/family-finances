namespace FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Domain.Enums;
using MediatR;

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

public record TransactionDto(
    Guid Id, Guid FamilyId, Guid AccountId, Guid UserId, Guid CategoryId,
    string Type, decimal Amount, string Currency, string Description,
    DateOnly TransactionDate, DateTime CreatedAt);
