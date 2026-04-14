namespace FinanceApp.Application.Features.Transactions.GetTransactions;
using FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Domain.Entities;
using MediatR;
using Supabase;
using static Supabase.Postgrest.Constants;

/// <summary>
/// Handles <see cref="GetTransactionsQuery"/>: retrieves transactions for a family with optional month/year filtering.
/// </summary>
public class GetTransactionsHandler(Client supabase)
    : IRequestHandler<GetTransactionsQuery, IReadOnlyList<TransactionDto>>
{
    public async Task<IReadOnlyList<TransactionDto>> Handle(
        GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactionsResponse = await supabase.From<Transaction>()
            .Where(t => t.FamilyId == request.FamilyId)
            .Get();

        var transactions = transactionsResponse.Models ?? new List<Transaction>();

        if (request.Year.HasValue)
            transactions = transactions.Where(t => t.TransactionDate.Year == request.Year.Value).ToList();
        if (request.Month.HasValue)
            transactions = transactions.Where(t => t.TransactionDate.Month == request.Month.Value).ToList();

        var categoryIds = transactions.Select(t => t.CategoryId).Distinct().ToList();
        // Where(c => categoryIds.Contains(c.Id)) no es soportado por el SDK (captura variable externa).
        // Usar Filter con Operator.In para generar: id=in.(guid1,guid2,...)
        var categoriesResponse = categoryIds.Any()
            ? await supabase.From<Category>()
                .Filter("id", Operator.In, categoryIds)
                .Get()
            : null;

        var categories = categoriesResponse?.Models?.ToDictionary(c => c.Id, c => c.Name)
            ?? new Dictionary<Guid, string>();

        return transactions
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => new TransactionDto(
                t.Id,
                t.FamilyId,
                t.AccountId,
                t.UserId,
                t.CategoryId,
                categories.GetValueOrDefault(t.CategoryId, "Unknown"),
                t.Type.ToString(),
                t.Amount,
                t.Currency,
                t.Description,
                t.TransactionDate,
                t.CreatedAt))
            .ToList();
    }
}
