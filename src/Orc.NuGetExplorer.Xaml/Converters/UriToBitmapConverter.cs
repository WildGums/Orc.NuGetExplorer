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

    public class UriToBitmapConverter : ValueConverterBase<Uri, BitmapImage>
    {
        private static readonly IconCache IconCache;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        static UriToBitmapConverter()
        {
            if (IconCache == null)
            {
                var appCacheProvider = ServiceLocator.Default.ResolveType<IApplicationCacheProvider>();

                IconCache = appCacheProvider.EnsureIconCache();
            }
        }

        protected override object Convert(Uri value, Type targetType, object parameter)
        {
            //get bitmap from stream cache
            try
            {
                return IconCache.GetFromCache(value);
            }
            catch (Exception e)
            {
                Log.Error("Error occured during value conversion", e);
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
