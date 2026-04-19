namespace FinanceApp.Application.Common;

/// <summary>
/// Resource keys for user-facing error messages. Values must match entries in
/// <c>FinanceApp.API/Resources/SharedResource.resx</c> (and its translations).
/// </summary>
public static class LocalizationKeys
{
    public const string Auth_UserNotFound           = nameof(Auth_UserNotFound);
    public const string Auth_InvalidCredentials     = nameof(Auth_InvalidCredentials);
    public const string Auth_EmailAlreadyRegistered = nameof(Auth_EmailAlreadyRegistered);
    public const string Auth_EmailNotVerified       = nameof(Auth_EmailNotVerified);
    public const string Auth_InvalidToken           = nameof(Auth_InvalidToken);
    public const string Auth_TokenExpired           = nameof(Auth_TokenExpired);
    public const string Auth_AlreadyVerified        = nameof(Auth_AlreadyVerified);
    public const string Auth_InvalidTotpCode        = nameof(Auth_InvalidTotpCode);
    public const string Auth_TwoFactorAlreadyEnabled = nameof(Auth_TwoFactorAlreadyEnabled);
    public const string Auth_TwoFactorNotEnabled    = nameof(Auth_TwoFactorNotEnabled);
    public const string Account_NoFamilyAssociated  = nameof(Account_NoFamilyAssociated);
    public const string Account_DuplicateName        = nameof(Account_DuplicateName);
    public const string Account_NotFound             = nameof(Account_NotFound);
    public const string Transaction_CategoryNotFound = nameof(Transaction_CategoryNotFound);
    public const string Transaction_NotFound         = nameof(Transaction_NotFound);
    public const string Family_NotFound              = nameof(Family_NotFound);
    public const string Family_InvalidInviteCode     = nameof(Family_InvalidInviteCode);
    public const string Family_NotOwner              = nameof(Family_NotOwner);

    // Password complexity
    public const string Password_RequiresUppercase   = nameof(Password_RequiresUppercase);
    public const string Password_RequiresLowercase   = nameof(Password_RequiresLowercase);
    public const string Password_RequiresDigit       = nameof(Password_RequiresDigit);
    public const string Password_RequiresSpecial     = nameof(Password_RequiresSpecial);

    // Auth — credential management
    public const string Auth_PasswordSameAsCurrent   = nameof(Auth_PasswordSameAsCurrent);
    public const string Auth_InvalidCurrentPassword  = nameof(Auth_InvalidCurrentPassword);
    public const string Auth_EmailAlreadyInUse       = nameof(Auth_EmailAlreadyInUse);
    public const string Auth_PasswordMismatch        = nameof(Auth_PasswordMismatch);

    // Auth — email change
    public const string Auth_InvalidEmailChangeToken = nameof(Auth_InvalidEmailChangeToken);
    public const string Auth_EmailChangeTokenExpired = nameof(Auth_EmailChangeTokenExpired);
}
