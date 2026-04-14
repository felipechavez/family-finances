namespace FinanceApp.Infrastructure.Settings;
using System.ComponentModel.DataAnnotations;

/// <summary>Configuration for sending email via the MailerSend SMTP relay.</summary>
public class MailerSendSettings
{
    /// <summary>SMTP host. Default: MailerSend relay.</summary>
    public string SmtpHost { get; set; } = "smtp.mailersend.net";

    /// <summary>SMTP port. Default: 587 (STARTTLS).</summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>SMTP username — generated in MailerSend dashboard under "SMTP".</summary>
    [Required]
    public string SmtpUser { get; set; } = string.Empty;

    /// <summary>SMTP password — generated alongside the SMTP username in MailerSend dashboard.</summary>
    [Required]
    public string SmtpPassword { get; set; } = string.Empty;

    /// <summary>Verified sender address (must match a verified domain in MailerSend).</summary>
    [Required]
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>Frontend base URL used to build links in emails (e.g. verification link).</summary>
    [Required]
    public string AppBaseUrl { get; set; } = "http://localhost:3000";
}
