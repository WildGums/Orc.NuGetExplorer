namespace Orc.NuGetExplorer.Converters
{
    using System;
    using Catel.MVVM.Converters;

    [System.Windows.Data.ValueConversion(typeof(Uri), typeof(string))]
    public class ExtendedUriToStringConverter : ValueConverterBase<Uri, string>
    {
        protected override object? Convert(Uri? value, Type targetType, object? parameter)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}
