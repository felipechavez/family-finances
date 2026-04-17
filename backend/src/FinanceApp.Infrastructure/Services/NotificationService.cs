namespace FinanceApp.Infrastructure.Services;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Persists and retrieves in-app notifications via Supabase.
/// </summary>
public sealed class NotificationService(
    Client supabase,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task CreateBudgetExceededAsync(
        Guid userId, Guid familyId, string categoryName,
        decimal spent, decimal limit, CancellationToken ct = default)
    {
        var pct = limit > 0 ? (int)Math.Round(spent / limit * 100) : 0;
        var n = Notification.Create(
            userId, familyId,
            type: "budget_exceeded",
            title: $"⚠ Límite superado: {categoryName}",
            body: $"Gastado ${spent:N0} de ${limit:N0} ({pct}%)");

        try
        {
            await supabase.From<Notification>().Insert(n);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create budget_exceeded notification for user {UserId}", userId);
        }
    }

    public async Task<IReadOnlyList<NotificationDto>> GetRecentAsync(Guid userId, CancellationToken ct = default)
    {
        var resp = await supabase.From<Notification>()
            .Where(n => n.UserId == userId)
            .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
            .Limit(30)
            .Get();

        return (resp.Models ?? [])
            .Select(n => new NotificationDto(n.Id, n.Type, n.Title, n.Body, n.IsRead, n.CreatedAt))
            .ToList()
            .AsReadOnly();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var resp = await supabase.From<Notification>()
            .Where(n => n.UserId == userId && !n.IsRead)
            .Get();

        return resp.Models?.Count ?? 0;
    }

    public async Task MarkReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default)
    {
        try
        {
            await supabase.From<Notification>()
                .Where(n => n.Id == notificationId && n.UserId == userId)
                .Set(n => n.IsRead, true)
                .Update();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to mark notification {Id} as read", notificationId);
        }
    }

    public async Task MarkAllReadAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            await supabase.From<Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .Set(n => n.IsRead, true)
                .Update();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to mark all notifications read for user {UserId}", userId);
        }
    }
}
