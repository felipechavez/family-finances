namespace FinanceApp.Application.Features.Reports.GetMonthlySummary;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Supabase;
using System.Text.Json;

/// <summary>
/// Handles <see cref="GetMonthlySummaryQuery"/>: computes income/expense totals grouped by category
/// for a given month. Results are cached in Redis for 5 minutes to reduce database load.
/// </summary>
public class GetMonthlySummaryHandler(Client supabase, IDistributedCache cache, ILogger<GetMonthlySummaryHandler> logger)
    : IRequestHandler<GetMonthlySummaryQuery, MonthlySummaryDto>
{
    public async Task<MonthlySummaryDto> Handle(GetMonthlySummaryQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"report:monthly:{request.FamilyId}:{request.Year}:{request.Month}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            logger.LogDebug("Monthly summary for family {FamilyId} {Year}/{Month} served from cache",
                request.FamilyId, request.Year, request.Month);
            return JsonSerializer.Deserialize<MonthlySummaryDto>(cached)!;
        }

        var transactionsResponse = await supabase.From<Transaction>()
            .Where(t => t.FamilyId == request.FamilyId)
            .Get();

        var transactions = transactionsResponse.Models ?? new List<Transaction>();
        transactions = transactions
            .Where(t => t.TransactionDate.Year == request.Year && t.TransactionDate.Month == request.Month)
            .ToList();

        var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var categoryGroups = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(t => t.Amount) })
            .ToList();

        var categoryIds = categoryGroups.Select(g => g.CategoryId).ToList();

        var categoriesResponse = categoryIds.Any()
            ? await supabase.From<Category>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, categoryIds.Select(id => id.ToString()).ToList())
                .Get()
            : null;

        var categories = categoriesResponse?.Models?.ToDictionary(c => c.Id, c => c.Name)
            ?? new Dictionary<Guid, string>();

        var budgetsResponse = categoryIds.Any()
            ? await supabase.From<Budget>()
                .Where(b => b.FamilyId == request.FamilyId && categoryIds.Contains(b.CategoryId))
                .Get()
            : null;

        var budgets = budgetsResponse?.Models?.ToDictionary(b => b.CategoryId, b => b.MonthlyLimit)
            ?? new Dictionary<Guid, decimal>();

        var expensesByCategory = categoryGroups
            .Select(g => new CategoryExpenseDto(
                g.CategoryId,
                categories.GetValueOrDefault(g.CategoryId, "Unknown"),
                g.Total,
                budgets.GetValueOrDefault(g.CategoryId, 0)))
            .OrderByDescending(c => c.Total)
            .ToList();

        var result = new MonthlySummaryDto(
            request.Year, request.Month, totalIncome, totalExpense,
            totalIncome - totalExpense, expensesByCategory);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
            cancellationToken);

        logger.LogInformation("Monthly summary computed for family {FamilyId} {Year}/{Month}: income={Income}, expense={Expense}",
            request.FamilyId, request.Year, request.Month, totalIncome, totalExpense);

        return result;
    }
}
