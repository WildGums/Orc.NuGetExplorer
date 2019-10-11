using Orc.NuGetExplorer.Cache;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.Web;

namespace Orc.NuGetExplorer.Services
{
    using Catel;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Cache;
    using NuGetExplorer.Providers;
    using NuGetExplorer.Web;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class PackageMetadataMediaDownloadService : IPackageMetadataMediaDownloadService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IconDownloader iconDownloader = new IconDownloader();

        private readonly IconCache iconCache;

        private string defaultIconUri = "pack://application:,,,/NuGetPackageManager;component/Resources/default-package-icon.png";

        public PackageMetadataMediaDownloadService(IApplicationCacheProvider appCacheProvider)
        {
            Argument.IsNotNull(() => appCacheProvider);

            iconCache = appCacheProvider.EnsureIconCache();
            iconCache.FallbackValue = new System.Windows.Media.Imaging.BitmapImage(new Uri(defaultIconUri));
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
                    var data = await iconDownloader.GetByUrlAsync(packageMetadata.IconUrl, webClient);
                    iconCache.SaveToCache(packageMetadata.IconUrl, data);
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
