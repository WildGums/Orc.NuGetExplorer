namespace Orc.NuGetExplorer.Converters;

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Catel.Logging;
using Catel.MVVM.Converters;
using Microsoft.Extensions.Logging;
using NuGetExplorer.Cache;
using NuGetExplorer.Providers;

[System.Windows.Data.ValueConversion(typeof(Uri), typeof(BitmapImage))]
public partial class UriToBitmapConverter : ValueConverterBase<Uri, BitmapImage>
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(UriToBitmapConverter));

    private readonly IconCache _iconCache;

    public UriToBitmapConverter(IApplicationCacheProvider applicationCacheProvider)
    {
        _iconCache = applicationCacheProvider.EnsureIconCache();
    }

    protected override object Convert(Uri? value, Type targetType, object? parameter)
    {
        try
        {
            if (value is null)
            {
                return DependencyProperty.UnsetValue;
            }

            //get bitmap from stream cache
            return _iconCache.GetFromCache(value) ?? DependencyProperty.UnsetValue;
        }
        catch (Exception ex)
        {
            // Don't list this as error, it's possible to have packages with missed icon.png
            Logger.LogWarning($"Error occurred during value conversion, {ex}");
            return DependencyProperty.UnsetValue;
        }
    }
}
