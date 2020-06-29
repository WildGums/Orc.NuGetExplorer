namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.MVVM.Converters;

    public class ExtendedUriToStringConverter : ValueConverterBase<Uri, string>
    {
        protected override object Convert(Uri value, Type targetType, object parameter)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}
