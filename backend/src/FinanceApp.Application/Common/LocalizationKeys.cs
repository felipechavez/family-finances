namespace FinanceApp.Application.Common;

/// <summary>
/// Resource keys for user-facing error messages. Values must match entries in
/// <c>FinanceApp.API/Resources/SharedResource.resx</c> (and its translations).
/// </summary>
public static class LocalizationKeys
{
    public const string Auth_InvalidCredentials     = nameof(Auth_InvalidCredentials);
    public const string Auth_EmailAlreadyRegistered = nameof(Auth_EmailAlreadyRegistered);
    public const string Account_NoFamilyAssociated  = nameof(Account_NoFamilyAssociated);
    public const string Transaction_CategoryNotFound = nameof(Transaction_CategoryNotFound);
    public const string Transaction_NotFound         = nameof(Transaction_NotFound);
}
