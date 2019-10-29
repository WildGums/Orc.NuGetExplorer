namespace Orc.NuGetExplorer.Converters
{
    using Catel.MVVM.Converters;
    using NuGet.Frameworks;
    using System;
    using System.Windows;

    public class NuGetFrameworkToVisibilityConverter : ValueConverterBase<NuGetFramework, Visibility>
    {
        protected override object Convert(NuGetFramework value, Type targetType, object parameter)
        {
            if (value == null || value.IsAny)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }
    }
}
