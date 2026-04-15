namespace FinanceApp.Infrastructure.Settings;
using System.ComponentModel.DataAnnotations;

/// <summary>Configuration for the SMTP email relay.</summary>
public class SmtpSettings
{
    /// <summary>SMTP server hostname.</summary>
    [Required]
    public string Host { get; set; } = string.Empty;

    /// <summary>SMTP port (465 = SSL, 587 = STARTTLS).</summary>
    public int Port { get; set; } = 465;

    /// <summary>SMTP username / sender address.</summary>
    [Required]
    public string User { get; set; } = string.Empty;

    /// <summary>SMTP account password.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>Frontend base URL used to build links in emails (e.g. verification link).</summary>
    [Required]
    public string AppBaseUrl { get; set; } = "http://localhost:3000";
}
