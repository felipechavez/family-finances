namespace FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handles <see cref="CreateTransactionCommand"/>: persists a new transaction and returns its projection.
/// </summary>
public class CreateTransactionHandler(IAppDbContext db, ILogger<CreateTransactionHandler> logger)
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    /// <inheritdoc/>
    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = Transaction.Create(
            request.FamilyId, request.AccountId, request.UserId,
            request.CategoryId, request.Type, request.Amount,
            request.Description, request.TransactionDate, request.Currency);

        db.Transactions.Add(tx);
        await db.SaveChangesAsync(cancellationToken);

        var categoryName = await db.Categories
            .AsNoTracking()
            .Where(c => c.Id == tx.CategoryId)
            .Select(c => c.Name)
            .SingleAsync(cancellationToken);

        logger.LogInformation("Transaction {TransactionId} ({Type} {Amount}) created for family {FamilyId}",
            tx.Id, tx.Type, tx.Amount, request.FamilyId);

        return ToDto(tx, categoryName);
    }

    /// <summary>Maps a <see cref="Transaction"/> and its category name to a <see cref="TransactionDto"/>.</summary>
    internal static TransactionDto ToDto(Transaction tx, string categoryName) => new(
        tx.Id, tx.FamilyId, tx.AccountId, tx.UserId, tx.CategoryId,
        categoryName, tx.Type.ToString(), tx.Amount, tx.Currency, tx.Description,
        tx.TransactionDate, tx.CreatedAt);
}
