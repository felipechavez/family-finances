namespace FinanceApp.Application.Common;

/// <summary>
/// Controls whether email address verification is enforced on registration and login.
/// Set <see cref="Enabled"/> to <c>false</c> in development to skip the Resend email step.
/// </summary>
public class EmailVerificationOptions
{
    public const string SectionName = "EmailVerification";

    /// <summary>
    /// When <c>false</c>: new users are pre-verified and the login gate is bypassed.
    /// Defaults to <c>true</c> (production behaviour).
    /// </summary>
    public bool Enabled { get; set; } = true;
}
