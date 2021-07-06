namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM.Converters;
    using NuGetExplorer.Cache;
    using NuGetExplorer.Providers;

    [System.Windows.Data.ValueConversion(typeof(Uri), typeof(BitmapImage))]
    public class UriToBitmapConverter : ValueConverterBase<Uri, BitmapImage>
    {
        private static readonly IconCache IconCache = InitCache();
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static IconCache InitCache()
        {
            var appCacheProvider = ServiceLocator.Default.ResolveType<IApplicationCacheProvider>();

            return appCacheProvider.EnsureIconCache();
        }

        protected override object Convert(Uri value, Type targetType, object parameter)
        {
            try
            {
                //get bitmap from stream cache
                return IconCache.GetFromCache(value);
            }
            catch (Exception ex)
            {
                // Don't list this as error, it's possible to have pacakges with missed icon.png
                Log.Warning($"Error occured during value conversion, {ex}");
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
