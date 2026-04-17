namespace FinanceApp.Infrastructure.BackgroundServices;
using FinanceApp.Application.Common.Email;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Hosted background service that sends a daily financial summary email to opted-in users.
/// Fires once per day at 20:00 local time.
/// </summary>
public sealed class DailySummaryWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<DailySummaryWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DailySummaryWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeUntilNextRun(TimeOnly.FromTimeSpan(TimeSpan.FromHours(20)));
            logger.LogInformation("DailySummaryWorker: next run in {Delay}", delay);

            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;

            await RunAsync(stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        logger.LogInformation("DailySummaryWorker: sending daily summaries");
        try
        {
            await using var scope  = scopeFactory.CreateAsyncScope();
            var supabase   = scope.ServiceProvider.GetRequiredService<Client>();
            var emailSvc   = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var today      = DateOnly.FromDateTime(DateTime.Today);
            var monthStart = new DateOnly(today.Year, today.Month, 1);
            var tomorrow   = today.AddDays(1);

            // 1. All families
            var familiesResp = await supabase.From<Family>().Get();
            var families     = familiesResp.Models ?? [];

            foreach (var family in families)
            {
                // 2. Members opted-in to daily summary
                var membersResp = await supabase.From<FamilyMember>()
                    .Where(m => m.FamilyId == family.Id)
                    .Get();

                var memberIds = (membersResp.Models ?? []).Select(m => m.UserId).ToList();
                if (memberIds.Count == 0) continue;

                // 3. Transactions of today for this family
                var txResp = await supabase.From<Transaction>()
                    .Where(t => t.FamilyId == family.Id
                             && t.TransactionDate >= today
                             && t.TransactionDate < tomorrow)
                    .Get();

                var todayTx = txResp.Models ?? [];

                // 4. Budgets for this family
                var budgetResp = await supabase.From<Budget>()
                    .Where(b => b.FamilyId == family.Id)
                    .Get();

                var budgets = (budgetResp.Models ?? [])
                    .ToDictionary(b => b.CategoryId, b => b.MonthlyLimit);

                // 5. Build per-category summary (month-to-date for context)
                var monthTxResp = await supabase.From<Transaction>()
                    .Where(t => t.FamilyId == family.Id
                             && t.Type == TransactionType.Expense
                             && t.TransactionDate >= monthStart
                             && t.TransactionDate < tomorrow)
                    .Get();

                var categoryTotals = (monthTxResp.Models ?? [])
                    .GroupBy(t => t.CategoryId)
                    .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

                // Enrich with category names
                var catIds = categoryTotals.Keys.ToList();
                var categories = new List<CategorySummaryRow>();

                if (catIds.Count > 0)
                {
                    var catResp = await supabase.From<Category>().Get();
                    var catNames = (catResp.Models ?? []).ToDictionary(c => c.Id, c => c.Name);

                    categories = catIds
                        .Select(id => new CategorySummaryRow(
                            catNames.GetValueOrDefault(id, id.ToString()),
                            categoryTotals[id],
                            budgets.GetValueOrDefault(id, 0m)))
                        .OrderByDescending(r => r.Amount)
                        .ToList();
                }

                var totalIncome   = todayTx.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                var totalExpenses = todayTx.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

                var data = new DailySummaryData(
                    family.Name, today, totalIncome, totalExpenses, categories);

                // 6. Send to each opted-in member
                foreach (var uid in memberIds)
                {
                    var userResp = await supabase.From<Users>()
                        .Where(u => u.Id == uid && u.DailySummaryEnabled)
                        .Get();

                    if (userResp.Model is not { } user) continue;

                    _ = emailSvc.SendDailySummaryAsync(user.Email, user.Name, data, ct);
                    logger.LogInformation("Daily summary queued for {Email}", user.Email);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DailySummaryWorker run failed");
        }
    }

    /// <summary>Returns the <see cref="TimeSpan"/> until the next occurrence of <paramref name="target"/>.</summary>
    private static TimeSpan TimeUntilNextRun(TimeOnly target)
    {
        var now  = DateTime.Now;
        var next = DateTime.Today.Add(target.ToTimeSpan());
        if (next <= now) next = next.AddDays(1);
        return next - now;
    }
}
