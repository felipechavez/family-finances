namespace FinanceApp.Infrastructure.Services;

using System.Text;
using FinanceApp.Application.Common.Email;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

/// <summary>
/// Sends transactional emails via the Resend API.
/// Failures are caught and logged — they never propagate to the caller.
/// </summary>
public sealed class ResendEmailService(
    IResend resend,
    IOptions<ResendSettings> settings,
    ILogger<ResendEmailService> logger) : IEmailService
{
    private readonly ResendSettings _cfg = settings.Value;

    /// <inheritdoc />
    public async Task SendVerificationEmailAsync(
        string toEmail,
        string toName,
        string verificationToken,
        CancellationToken ct = default)
    {
        var link = $"{_cfg.AppBaseUrl}/auth/verify-email?token={Uri.EscapeDataString(verificationToken)}";
        var html = TemplateLoader.Load("verification.html")
            .Replace("{{LOGO_URL}}", $"{_cfg.AppBaseUrl}/logo.png")
            .Replace("{{NOMBRE}}", toName)
            .Replace("{{LINK}}", link);

        await SendAsync(toEmail, "Verifica tu email — DomusPay", html, nameof(SendVerificationEmailAsync), ct);
    }

    /// <inheritdoc />
    public async Task SendDailySummaryAsync(
        string toEmail,
        string toName,
        DailySummaryData data,
        CancellationToken ct = default)
    {
        var balance = data.TotalIncome - data.TotalExpenses;
        var balanceColor = balance >= 0 ? "#059669" : "#dc2626";

        var categoryRows = BuildCategoryRows(data);

        var html = TemplateLoader.Load("daily-summary.html")
            .Replace("{{LOGO_URL}}", $"{_cfg.AppBaseUrl}/logo.png")
            .Replace("{{NOMBRE}}", toName)
            .Replace("{{FECHA}}", data.Date.ToString("dd/MM/yyyy"))
            .Replace("{{FAMILIA}}", data.FamilyName)
            .Replace("{{INGRESOS}}", $"${data.TotalIncome:N0}")
            .Replace("{{GASTOS}}", $"${data.TotalExpenses:N0}")
            .Replace("{{BALANCE}}", $"${balance:N0}")
            .Replace("{{BALANCE_COLOR}}", balanceColor)
            .Replace("{{CATEGORY_ROWS}}", categoryRows);

        await SendAsync(
            toEmail,
            $"Resumen del día {data.Date:dd/MM/yyyy} — {data.FamilyName}",
            html,
            nameof(SendDailySummaryAsync),
            ct);
    }

    /// <inheritdoc />
    public async Task SendBudgetAlertAsync(
        string toEmail,
        string toName,
        string categoryName,
        decimal spent,
        decimal limit,
        CancellationToken ct = default)
    {
        var pct = limit > 0 ? (int)Math.Round(spent / limit * 100) : 0;
        var html = TemplateLoader.Load("budget-alert.html")
            .Replace("{{LOGO_URL}}", $"{_cfg.AppBaseUrl}/logo.png")
            .Replace("{{NOMBRE}}", toName)
            .Replace("{{CATEGORIA}}", categoryName)
            .Replace("{{GASTADO}}", $"${spent:N0}")
            .Replace("{{LIMITE}}", $"${limit:N0}")
            .Replace("{{PORCENTAJE}}", pct.ToString());

        await SendAsync(toEmail, $"⚠ Límite de gasto superado: {categoryName}", html, nameof(SendBudgetAlertAsync), ct);
    }

    /// <inheritdoc />
    public async Task SendEmailChangeConfirmationAsync(
        string toEmail,
        string toName,
        string changeToken,
        CancellationToken ct = default)
    {
        var link = $"{_cfg.AppBaseUrl}/auth/verify-email-change?token={Uri.EscapeDataString(changeToken)}";
        var html = TemplateLoader.Load("email-change-confirmation.html")
            .Replace("{{LOGO_URL}}", $"{_cfg.AppBaseUrl}/logo.png")
            .Replace("{{NOMBRE}}", toName)
            .Replace("{{LINK}}", link);

        await SendAsync(toEmail, "Confirma tu nuevo correo — DomusPay", html, nameof(SendEmailChangeConfirmationAsync), ct);
    }

    /// <inheritdoc />
    public async Task SendEmailChangeNotificationAsync(
        string toEmail,
        string toName,
        CancellationToken ct = default)
    {
        var html = TemplateLoader.Load("email-change-notification.html")
            .Replace("{{LOGO_URL}}", $"{_cfg.AppBaseUrl}/logo.png")
            .Replace("{{NOMBRE}}", toName);

        await SendAsync(toEmail, "Solicitud de cambio de correo — DomusPay", html, nameof(SendEmailChangeNotificationAsync), ct);
    }

    // ── private helpers ────────────────────────────────────────────────────────

    private async Task SendAsync(string toEmail, string subject, string htmlBody, string operation, CancellationToken ct)
    {
        try
        {
            var message = new EmailMessage
            {
                From = _cfg.FromAddress,
                Subject = subject,
                HtmlBody = htmlBody,
            };
            message.To.Add(toEmail);

            await resend.EmailSendAsync(message, ct);
            logger.LogInformation("{Operation} sent to {Email}", operation, toEmail);
        }
        catch (Exception ex)
        {
            // Email failures must never crash the main flow — log and continue.
            logger.LogError(ex, "{Operation} failed for {Email}", operation, toEmail);
        }
    }

    private static string BuildCategoryRows(DailySummaryData data)
    {
        if (data.Categories.Count == 0)
            return "<p style='color:#9999bb;font-size:13px'>Sin gastos registrados hoy.</p>";

        var rows = new StringBuilder();
        foreach (var cat in data.Categories)
        {
            var exceeded = cat.MonthlyLimit > 0 && cat.Amount > cat.MonthlyLimit;
            var amountColor = exceeded ? "#dc2626" : "#1a1a2e";
            var limitCell = cat.MonthlyLimit > 0
                ? $"<td style='padding:10px 12px;color:#555577;font-size:14px'>${cat.MonthlyLimit:N0}</td>"
                : "<td style='padding:10px 12px;color:#9999bb;font-size:14px'>—</td>";

            rows.Append($"""
                <tr style="border-top:1px solid #e8e5ff">
                  <td style="padding:10px 12px;color:#1a1a2e;font-size:14px">{cat.CategoryName}</td>
                  <td style="padding:10px 12px;color:{amountColor};font-weight:600;font-size:14px">${cat.Amount:N0}</td>
                  {limitCell}
                </tr>
                """);
        }

        return $"""
            <table width="100%" cellpadding="0" cellspacing="0" style="border:1px solid #e8e5ff;border-radius:12px;overflow:hidden">
              <thead>
                <tr style="background:#f8f7ff">
                  <th style="padding:10px 12px;text-align:left;font-size:11px;color:#9999bb;font-weight:600">CATEGORÍA</th>
                  <th style="padding:10px 12px;text-align:left;font-size:11px;color:#9999bb;font-weight:600">GASTADO</th>
                  <th style="padding:10px 12px;text-align:left;font-size:11px;color:#9999bb;font-weight:600">LÍMITE</th>
                </tr>
              </thead>
              <tbody>{rows}</tbody>
            </table>
            """;
    }
}
