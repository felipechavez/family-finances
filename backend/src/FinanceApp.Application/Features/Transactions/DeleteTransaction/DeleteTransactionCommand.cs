namespace FinanceApp.Application.Features.Transactions.DeleteTransaction;
using MediatR;

public record DeleteTransactionCommand(Guid TransactionId, Guid FamilyId) : IRequest;
