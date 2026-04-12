using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace FinanceApp.API.Resources
{
    public class LocalizedFluentValidator
    {
        public static string GetLocalizedValidationErrors(
            ValidationException ve,
            IStringLocalizer<SharedResource> localizer)
        {
            var errors = new List<string>();

            foreach (var error in ve.Errors)
            {
                // Intentar localizar el mensaje de error
                var key = error.PropertyName;
                var localizedMessage = localizer[key].Value;

                // Si no existe localización, usar el mensaje original
                if (localizedMessage == key)
                {
                    localizedMessage = error.ErrorMessage;
                }

                errors.Add(localizedMessage);
            }

            return string.Join("; ", errors);
        }
    }
}
