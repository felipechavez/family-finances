namespace FinanceApp.Application.Common.Interfaces;
using FinanceApp.Application.Common.Email;

/// <summary>
/// Sends transactional emails to users.
/// All methods are fire-and-forward; callers should not block on email delivery.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email with a verification link to the given address.
    /// </summary>
    Task SendVerificationEmailAsync(
        string toEmail,
        string toName,
        string verificationToken,
        CancellationToken ct = default);

    /// <summary>
    /// Sends a daily summary email with the family's income, expenses and budget status.
    /// </summary>
    Task SendDailySummaryAsync(
        string toEmail,
        string toName,
        DailySummaryData data,
        CancellationToken ct = default);

    /// <summary>
    /// Sends a budget-exceeded alert when a category spending surpasses its monthly limit.
    /// </summary>
    Task SendBudgetAlertAsync(
        string toEmail,
        string toName,
        string categoryName,
        decimal spent,
        decimal limit,
        CancellationToken ct = default);
}
