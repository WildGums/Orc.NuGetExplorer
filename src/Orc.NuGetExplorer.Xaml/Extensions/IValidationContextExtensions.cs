namespace Orc.NuGetExplorer
{
    using System.Linq;
    using Catel.Data;

    public static class IValidationContextExtensions
    {
        public static string[]? GetAlertMessages(this IValidationContext validationContext, string validationTag)
        {
            var stringLines = validationContext.GetErrors(validationTag).Select(s => " - " + s.Message).ToArray();

            if (stringLines is null)
            {
                return null;
            }

            var valuableLines = stringLines.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            if (!valuableLines.Any())
            {
                return null;
            }

            return valuableLines;
        }
    }
}
