namespace Orc.NuGetExplorer.Converters
{
    using Catel.MVVM.Converters;
    using NuGet.Versioning;
    using System;

    public class NuGetVersionToStringConverter : ValueConverterBase<NuGetVersion, string>
    {
        protected override object Convert(NuGetVersion value, Type targetType, object parameter)
        {
            return value?.ToString() ?? Constants.NotInstalled;
        }
    }
}
