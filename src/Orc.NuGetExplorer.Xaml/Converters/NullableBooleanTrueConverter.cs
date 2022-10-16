namespace Orc.NuGetExplorer.Converters
{
    using System;
    using Catel.MVVM.Converters;

    [System.Windows.Data.ValueConversion(typeof(bool?), typeof(bool))]
    public class NullableBooleanTrueConverter : ValueConverterBase<bool?>
    {
        protected override object Convert(bool? value, Type targetType, object? parameter)
        {
            return value ?? true;
        }

        protected override object ConvertBack(object? value, Type targetType, object? parameter)
        {
            return (bool?)value;
        }
    }
}
