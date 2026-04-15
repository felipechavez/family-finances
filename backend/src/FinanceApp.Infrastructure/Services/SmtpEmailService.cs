namespace FinanceApp.Infrastructure.Services;
using System.Text;
using FinanceApp.Application.Common.Email;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

/// <summary>
/// Sends transactional emails via the configured SMTP relay using SSL on port 465.
/// </summary>
public sealed class SmtpEmailService(
    IOptions<SmtpSettings> settings,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly SmtpSettings _cfg = settings.Value;

    /// <inheritdoc />
    public async Task SendVerificationEmailAsync(
        string toEmail,
        string toName,
        string verificationToken,
        CancellationToken ct = default)
    {
        var link = $"{_cfg.AppBaseUrl}/auth/verify-email?token={Uri.EscapeDataString(verificationToken)}";

        var message = BuildMessage(
            toEmail,
            toName,
            "Verifica tu email — FinanzasApp",
            BuildVerificationHtml(toName, link));

        await SendAsync(message, nameof(SendVerificationEmailAsync), toEmail, ct);
    }

    /// <inheritdoc />
    public async Task SendDailySummaryAsync(
        string toEmail,
        string toName,
        DailySummaryData data,
        CancellationToken ct = default)
    {
        var message = BuildMessage(
            toEmail,
            toName,
            $"Resumen del día {data.Date:dd/MM/yyyy} — {data.FamilyName}",
            BuildDailySummaryHtml(toName, data));

        await SendAsync(message, nameof(SendDailySummaryAsync), toEmail, ct);
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
        var message = BuildMessage(
            toEmail,
            toName,
            $"⚠ Límite de gasto superado: {categoryName}",
            BuildBudgetAlertHtml(toName, categoryName, spent, limit));

        await SendAsync(message, nameof(SendBudgetAlertAsync), toEmail, ct);
    }

    // ── private helpers ────────────────────────────────────────────────────────

    private MimeMessage BuildMessage(string toEmail, string toName, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("FinanzasApp", _cfg.User));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };
        return message;
    }

    private async Task SendAsync(MimeMessage message, string operation, string toEmail, CancellationToken ct)
    {
        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.SslOnConnect, ct);
            await smtp.AuthenticateAsync(_cfg.User, _cfg.Password, ct);
            await smtp.SendAsync(message, ct);
            await smtp.DisconnectAsync(true, ct);
            logger.LogInformation("{Operation} sent to {Email}", operation, toEmail);
        }
        catch (Exception ex)
        {
            // Email failures must never crash the main flow — log and continue.
            logger.LogError(ex, "{Operation} failed for {Email}", operation, toEmail);
        }
    }

    private static string BuildVerificationHtml(string name, string link) => $"""
        <!DOCTYPE html>
        <html lang="es">
        <body style="font-family:sans-serif;background:#0c0c18;color:#f0eeff;padding:32px">
          <div style="max-width:480px;margin:auto;background:#13131f;border:1px solid #2a2a40;border-radius:16px;padding:32px">
            <h1 style="color:#a78bfa;font-size:22px;margin:0 0 8px">FinanzasApp Familiar</h1>
            <h2 style="font-size:18px;font-weight:600;margin:0 0 20px">Verifica tu dirección de email</h2>
            <p style="color:#c4b5fd;margin:0 0 24px">Hola {name}, haz clic en el botón para activar tu cuenta:</p>
            <a href="{link}"
               style="display:inline-block;background:linear-gradient(135deg,#7c3aed,#4f46e5);color:#fff;
                      text-decoration:none;padding:14px 28px;border-radius:12px;font-weight:600;font-size:15px">
              Verificar email
            </a>
            <p style="color:#6b6b8a;font-size:12px;margin:24px 0 0">
              Este enlace expira en 24 horas. Si no creaste esta cuenta, ignora este correo.
            </p>
          </div>
        </body>
        </html>
        """;

    private static string BuildBudgetAlertHtml(string name, string category, decimal spent, decimal limit)
    {
        var pct = limit > 0 ? (int)Math.Round(spent / limit * 100) : 0;
        return $"""
            <!DOCTYPE html>
            <html lang="es">
            <body style="font-family:sans-serif;background:#0c0c18;color:#f0eeff;padding:32px">
              <div style="max-width:480px;margin:auto;background:#13131f;border:1px solid #2a2a40;border-radius:16px;padding:32px">
                <h1 style="color:#a78bfa;font-size:22px;margin:0 0 8px">FinanzasApp Familiar</h1>
                <h2 style="font-size:18px;font-weight:600;margin:0 0 20px;color:#f87171">⚠ Límite de gasto superado</h2>
                <p style="color:#c4b5fd;margin:0 0 16px">Hola {name},</p>
                <p style="margin:0 0 20px">
                  La categoría <strong style="color:#f0eeff">{category}</strong> ha superado su límite mensual.
                </p>
                <table style="width:100%;border-collapse:collapse;margin-bottom:24px">
                  <tr>
                    <td style="padding:8px 0;color:#8888aa">Gastado</td>
                    <td style="padding:8px 0;text-align:right;color:#f87171;font-weight:700">${spent:N0}</td>
                  </tr>
                  <tr>
                    <td style="padding:8px 0;color:#8888aa">Límite</td>
                    <td style="padding:8px 0;text-align:right;color:#a78bfa;font-weight:700">${limit:N0}</td>
                  </tr>
                  <tr>
                    <td style="padding:8px 0;color:#8888aa">Uso</td>
                    <td style="padding:8px 0;text-align:right;color:#f87171;font-weight:700">{pct}%</td>
                  </tr>
                </table>
                <p style="color:#6b6b8a;font-size:12px;margin:0">Revisa tu presupuesto en FinanzasApp.</p>
              </div>
            </body>
            </html>
            """;
    }

    private static string BuildDailySummaryHtml(string name, DailySummaryData data)
    {
        var balance = data.TotalIncome - data.TotalExpenses;
        var balanceColor = balance >= 0 ? "#34d399" : "#f87171";

        var rows = new StringBuilder();
        foreach (var cat in data.Categories)
        {
            var exceeded = cat.MonthlyLimit > 0 && cat.Amount > cat.MonthlyLimit;
            var amountColor = exceeded ? "#f87171" : "#f0eeff";
            var limitCell = cat.MonthlyLimit > 0
                ? $"<td style='padding:8px 12px;color:#6b6b8a'>${cat.MonthlyLimit:N0}</td>"
                : "<td style='padding:8px 12px;color:#6b6b8a'>—</td>";

            rows.Append($"""
                <tr style="border-top:1px solid #2a2a40">
                  <td style="padding:8px 12px;color:#f0eeff">{cat.CategoryName}</td>
                  <td style="padding:8px 12px;color:{amountColor};font-weight:600">${cat.Amount:N0}</td>
                  {limitCell}
                </tr>
                """);
        }

        return $"""
            <!DOCTYPE html>
            <html lang="es">
            <body style="font-family:sans-serif;background:#0c0c18;color:#f0eeff;padding:32px">
              <div style="max-width:560px;margin:auto;background:#13131f;border:1px solid #2a2a40;border-radius:16px;padding:32px">
                <h1 style="color:#a78bfa;font-size:22px;margin:0 0 4px">FinanzasApp Familiar</h1>
                <p style="color:#6b6b8a;font-size:13px;margin:0 0 24px">Resumen del {data.Date:dd/MM/yyyy} · {data.FamilyName}</p>
                <p style="margin:0 0 20px;color:#c4b5fd">Hola {name}, aquí tienes el resumen de hoy:</p>

                <!-- Summary cards -->
                <div style="display:flex;gap:12px;margin-bottom:24px">
                  <div style="flex:1;background:#18182a;border-radius:12px;padding:16px;text-align:center">
                    <p style="font-size:11px;color:#6b6b8a;margin:0 0 4px;text-transform:uppercase;letter-spacing:1px">Ingresos</p>
                    <p style="font-size:20px;font-weight:700;color:#34d399;margin:0">${data.TotalIncome:N0}</p>
                  </div>
                  <div style="flex:1;background:#18182a;border-radius:12px;padding:16px;text-align:center">
                    <p style="font-size:11px;color:#6b6b8a;margin:0 0 4px;text-transform:uppercase;letter-spacing:1px">Gastos</p>
                    <p style="font-size:20px;font-weight:700;color:#f87171;margin:0">${data.TotalExpenses:N0}</p>
                  </div>
                  <div style="flex:1;background:#18182a;border-radius:12px;padding:16px;text-align:center">
                    <p style="font-size:11px;color:#6b6b8a;margin:0 0 4px;text-transform:uppercase;letter-spacing:1px">Balance</p>
                    <p style="font-size:20px;font-weight:700;color:{balanceColor};margin:0">${balance:N0}</p>
                  </div>
                </div>

                <!-- Category breakdown -->
                {(data.Categories.Count > 0 ? $"""
                <table style="width:100%;border-collapse:collapse">
                  <thead>
                    <tr style="background:#18182a">
                      <th style="padding:10px 12px;text-align:left;font-size:11px;color:#8888aa;font-weight:600">CATEGORÍA</th>
                      <th style="padding:10px 12px;text-align:left;font-size:11px;color:#8888aa;font-weight:600">GASTADO</th>
                      <th style="padding:10px 12px;text-align:left;font-size:11px;color:#8888aa;font-weight:600">LÍMITE</th>
                    </tr>
                  </thead>
                  <tbody>{rows}</tbody>
                </table>
                """ : "<p style='color:#6b6b8a;font-size:13px'>Sin gastos registrados hoy.</p>")}

                <p style="color:#6b6b8a;font-size:12px;margin:24px 0 0">
                  Para desactivar este resumen diario, ve a Configuración en la app.
                </p>
              </div>
            </body>
            </html>
            """;
    }
}
