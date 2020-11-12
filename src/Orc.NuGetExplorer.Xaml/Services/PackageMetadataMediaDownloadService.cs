namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Catel;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Cache;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Web;

    public class PackageMetadataMediaDownloadService : IPackageMetadataMediaDownloadService, IImageResolveService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IconDownloader IconDownloader = new IconDownloader();

        private readonly object _lockObject = new object();
        private readonly IconCache _iconCache;

        private readonly string _defaultIconUri = "pack://application:,,,/Orc.NuGetExplorer.Xaml;component/Resources/Images/default-package-icon.png";

        public PackageMetadataMediaDownloadService(IApplicationCacheProvider appCacheProvider)
        {
            Argument.IsNotNull(() => appCacheProvider);

            _iconCache = appCacheProvider.EnsureIconCache();
            _iconCache.FallbackValue = new System.Windows.Media.Imaging.BitmapImage(new Uri(_defaultIconUri));
        }

        public async Task DownloadMediaForMetadataAsync(IPackageSearchMetadata packageMetadata)
        {
            try
            {
                //skip if already in cache
                if (_iconCache.IsCached(packageMetadata.IconUrl))
                {
                    return;
                }

                if (packageMetadata.IconUrl.IsLoopback)
                {
                    // No need to cache local files
                    return;
                }

                await DownloadFromAsync(packageMetadata.IconUrl);
            }
            catch (WebException ex)
            {
                Log.Error(ex);
                return;
            }
        }

        private async Task DownloadFromAsync(Uri uri)
        {
            if (uri == null)
            {
                //default picture
                return;
            }

            using (var webClient = new WebClient())
            {
                var data = await IconDownloader.GetByUrlAsync(uri, webClient);
                _iconCache.SaveToCache(uri, data);
            }
        }

        private void DownloadFrom(Uri uri)
        {
            if (uri == null)
            {
                //default picture
                return;
            }

            using (var webClient = new WebClient())
            {
                var data = IconDownloader.GetByUrl(uri, webClient);
                _iconCache.SaveToCache(uri, data);
            }
        }

        #region IImageResolveService

        public ImageSource ResolveImageFromUri(Uri uri, string defaultUrl = null)
        {
            try
            {
                if (uri != null && Uri.IsWellFormedUriString(uri.ToString(), UriKind.RelativeOrAbsolute))
                {
                    return GetFromCacheOrFetch(uri);
                }
            }
            catch (WebException ex)
            {
                Log.Error(ex);
            }
            return new BitmapImage(new Uri(_defaultIconUri));
        }

        private ImageSource GetFromCacheOrFetch(Uri uri)
        {
            if (!_iconCache.IsCached(uri))
            {
                DownloadFrom(uri);
            }

            return _iconCache.GetFromCache(uri);
        }

        public async Task<ImageSource> ResolveImageFromUriAsync(Uri uri, string defaultUrl = null)
        {
            try
            {
                if (uri != null && Uri.IsWellFormedUriString(uri.ToString(), UriKind.RelativeOrAbsolute))
                {
                    return await GetFromCacheOrFetchAsync(uri);
                }
            }
            catch (WebException ex)
            {
                Log.Error(ex);
            }

            return new BitmapImage(new Uri(defaultUrl));
        }

        private async Task<ImageSource> GetFromCacheOrFetchAsync(Uri uri)
        {
            if (!_iconCache.IsCached(uri))
            {
                await DownloadFromAsync(uri);
            }

            return _iconCache.GetFromCache(uri);
        }

        #endregion
    }
}
