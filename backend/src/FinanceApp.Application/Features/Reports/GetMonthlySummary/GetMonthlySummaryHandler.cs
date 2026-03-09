namespace FinanceApp.Application.Features.Reports.GetMonthlySummary;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class GetMonthlySummaryHandler(IAppDbContext db, IDistributedCache cache)
    : IRequestHandler<GetMonthlySummaryQuery, MonthlySummaryDto>
{
    public async Task<MonthlySummaryDto> Handle(GetMonthlySummaryQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"report:monthly:{request.FamilyId}:{request.Year}:{request.Month}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
            return JsonSerializer.Deserialize<MonthlySummaryDto>(cached)!;

        var transactions = await db.Transactions
            .AsNoTracking()
            .Where(t => t.FamilyId == request.FamilyId
                && t.TransactionDate.Year == request.Year
                && t.TransactionDate.Month == request.Month)
            .ToListAsync(cancellationToken);

        var totalIncome  = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var categoryGroups = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(t => t.Amount) })
            .ToList();

        var categoryIds = categoryGroups.Select(g => g.CategoryId).ToList();

        var categories = await db.Categories
            .AsNoTracking()
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var budgets = await db.Budgets
            .AsNoTracking()
            .Where(b => b.FamilyId == request.FamilyId && categoryIds.Contains(b.CategoryId))
            .ToDictionaryAsync(b => b.CategoryId, b => b.MonthlyLimit, cancellationToken);

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

        return result;
    }
}
