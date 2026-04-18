namespace FinanceApp.Infrastructure.Settings;
using System.ComponentModel.DataAnnotations;

/// <summary>Configuration for the Resend transactional email service.</summary>
public class ResendSettings
{
    /// <summary>Resend API key (re_...).</summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Verified sender address configured in the Resend dashboard.</summary>
    [Required]
    public string FromAddress { get; set; } = "noreply@domuspay.cl";

    /// <summary>Frontend base URL used to build links in emails (e.g. verification link).</summary>
    [Required]
    public string AppBaseUrl { get; set; } = "http://localhost:3000";
}
