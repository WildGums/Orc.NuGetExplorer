namespace Orc.NuGetExplorer.Converters;

using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using Catel.MVVM.Converters;

[ValueConversion(typeof(ICollection), typeof(Visibility))]
public class EmptyCollectionToVisibleConverter : CollectionToCollapsingVisibilityConverter
{
    protected override object? Convert(object? value, Type targetType, object? parameter)
    {
        var isVisible = IsVisible(value, targetType, parameter);
        return isVisible ? Visibility.Collapsed : Visibility.Visible; // reverse visibility;
    }
}