namespace Orc.NuGetExplorer.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;

    public static class IValidationContextExtensions
    {
        public static string[] GetAlertMessages(this IValidationContext validationContext, string validationTag)
        {
            Argument.IsNotNull(() => validationContext);

            var stringLines = validationContext.GetErrors(validationTag).Select(s => " - " + s.Message).ToArray();
            
            if (stringLines == null)
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
