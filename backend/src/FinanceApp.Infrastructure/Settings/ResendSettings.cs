namespace FinanceApp.Infrastructure.Settings;
using System.ComponentModel.DataAnnotations;

/// <summary>Configuration for the Resend email service.</summary>
public class ResendSettings
{
    /// <summary>Resend API token (re_...).</summary>
    [Required]
    public string ApiToken { get; set; } = string.Empty;

    /// <summary>
    /// Verified sender address. Use "onboarding@resend.dev" for local development
    /// before setting up a custom domain in Resend.
    /// </summary>
    [Required]
    public string FromAddress { get; set; } = "onboarding@resend.dev";

    /// <summary>Frontend base URL used to build links in emails (e.g. verification link).</summary>
    [Required]
    public string AppBaseUrl { get; set; } = "http://localhost:3000";
}
