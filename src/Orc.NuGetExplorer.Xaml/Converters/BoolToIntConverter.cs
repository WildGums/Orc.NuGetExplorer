namespace Orc.NuGetExplorer.Converters
{
    using System;
    using Catel.MVVM.Converters;

    public class BoolToIntConverter : ValueConverterBase<bool, int>
    {
        protected override object Convert(bool value, Type targetType, object parameter)
        {
            return value ? 1 : 0;
        }
    }
}
