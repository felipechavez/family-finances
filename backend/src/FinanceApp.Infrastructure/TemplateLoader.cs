namespace FinanceApp.Infrastructure;

using System.Reflection;

internal static class TemplateLoader
{
    private static readonly Assembly _assembly = typeof(TemplateLoader).Assembly;

    internal static string Load(string templateName)
    {
        var resourceName = $"FinanceApp.Infrastructure.Templates.{templateName}";
        using var stream = _assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Email template '{resourceName}' not found as embedded resource.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
