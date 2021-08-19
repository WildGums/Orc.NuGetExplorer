namespace Orc.NuGetExplorer
{
    using System.Linq;
    using Catel;
    using Catel.Data;

    public static class IValidationContextExtensions
    {
        public static string[] GetAlertMessages(this IValidationContext validationContext, string validationTag)
        {
            Argument.IsNotNull(() => validationContext);

            var stringLines = validationContext.GetErrors(validationTag).Select(s => " - " + s.Message).ToArray();

            if (stringLines is null)
            {
                return new string[0];
            }

            var valuableLines = stringLines.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            return valuableLines;
        }
    }
}
