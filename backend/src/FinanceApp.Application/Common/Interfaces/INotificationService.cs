namespace FinanceApp.Application.Common.Interfaces;

public record NotificationDto(
    Guid Id,
    string Type,
    string Title,
    string? Body,
    bool IsRead,
    DateTime CreatedAt);

/// <summary>
/// Manages in-app notifications (creation, retrieval, read-status).
/// </summary>
public interface INotificationService
{
    /// <summary>Creates a budget-exceeded notification for the given user.</summary>
    Task CreateBudgetExceededAsync(
        Guid userId,
        Guid familyId,
        string categoryName,
        decimal spent,
        decimal limit,
        CancellationToken ct = default);

    /// <summary>Returns the 30 most recent notifications for the user, newest first.</summary>
    Task<IReadOnlyList<NotificationDto>> GetRecentAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Returns the count of unread notifications for the user.</summary>
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Marks a single notification as read (only if it belongs to the user).</summary>
    Task MarkReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default);

    /// <summary>Marks all notifications for the user as read.</summary>
    Task MarkAllReadAsync(Guid userId, CancellationToken ct = default);
}
