namespace FinanceApp.Domain.Common;

/// <summary>
/// Domain exception that carries a localization resource key and an HTTP status code.
/// The API layer resolves the key to a localized message before returning the response.
/// </summary>
public sealed class AppException : Exception
{
    public string ResourceKey { get; }
    public int StatusCode { get; }
    public object[] Args { get; }

    public AppException(string resourceKey, int statusCode = 400, params object[] args)
        : base(resourceKey)
    {
        ResourceKey = resourceKey;
        StatusCode = statusCode;
        Args = args;
    }
}
