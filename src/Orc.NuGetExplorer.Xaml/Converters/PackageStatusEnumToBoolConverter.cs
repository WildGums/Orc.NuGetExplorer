namespace Orc.NuGetExplorer.Converters
{
    using Catel.MVVM.Converters;
    using NuGetExplorer.Enums;
    using System;

    public class PackageStatusEnumToBoolConverter : ValueConverterBase<PackageStatus, bool>
    {
        protected override object Convert(PackageStatus value, Type targetType, object parameter)
        {
            return Math.Abs((int)value) <= 1;
        }
    }
}
