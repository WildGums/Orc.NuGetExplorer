namespace Orc.NuGetExplorer.Services
{
    using Orc.NuGetExplorer.Cache;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Web;
    using Catel;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class PackageMetadataMediaDownloadService : IPackageMetadataMediaDownloadService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IconDownloader IconDownloader = new IconDownloader();

        private readonly IconCache _iconCache;

        private readonly string _defaultIconUri = "pack://application:,,,/Orc.NuGetExplorer.Example.NET;component/Resources/default-package-icon.png";

        public PackageMetadataMediaDownloadService(IApplicationCacheProvider appCacheProvider)
        {
            Argument.IsNotNull(() => appCacheProvider);

            _iconCache = appCacheProvider.EnsureIconCache();
            _iconCache.FallbackValue = new System.Windows.Media.Imaging.BitmapImage(new Uri(_defaultIconUri));
        }

        public async Task DownloadFromAsync(IPackageSearchMetadata packageMetadata)
        {
            try
            {
                if (packageMetadata.IconUrl == null)
                {
                    //default picture
                    return;
                }

                using (var webClient = new WebClient())
                {
                    var data = await IconDownloader.GetByUrlAsync(packageMetadata.IconUrl, webClient);
                    _iconCache.SaveToCache(packageMetadata.IconUrl, data);
                }
                //store to cache
            }
            catch (WebException ex)
            {
                Log.Error(ex);
                return;
            }
        }
    }
}
