namespace Orc.NuGetExplorer.Converters;

using System;
using Catel.MVVM.Converters;
using NuGetExplorer.Enums;

[System.Windows.Data.ValueConversion(typeof(PackageStatus), typeof(bool))]
public class PackageStatusEnumToBoolConverter : ValueConverterBase<PackageStatus, bool>
{
    protected override object Convert(PackageStatus value, Type targetType, object? parameter)
    {
        return Math.Abs((int)value) <= 1;
    }
}