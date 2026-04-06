namespace FinanceApp.Application.Features.Transactions.DeleteTransaction;
using MediatR;

/// <summary>Command to permanently delete a transaction. The transaction must belong to the specified family.</summary>
/// <param name="TransactionId">The identifier of the transaction to delete.</param>
/// <param name="FamilyId">The family scope used to authorise the deletion.</param>
public record DeleteTransactionCommand(Guid TransactionId, Guid FamilyId) : IRequest;
