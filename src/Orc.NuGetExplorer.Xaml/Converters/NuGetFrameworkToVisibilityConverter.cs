namespace Orc.NuGetExplorer.Converters;

using System;
using System.Windows;
using Catel.MVVM.Converters;
using NuGet.Frameworks;

[System.Windows.Data.ValueConversion(typeof(NuGetFramework), typeof(Visibility))]
public class NuGetFrameworkToVisibilityConverter : ValueConverterBase<NuGetFramework, Visibility>
{
    protected override object Convert(NuGetFramework? value, Type targetType, object? parameter)
    {
        if (value is null || value.IsAny)
        {
            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }
}