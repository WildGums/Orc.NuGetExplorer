namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Cache;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Web;

    public class PackageMetadataMediaDownloadService : IPackageMetadataMediaDownloadService, IImageResolveService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IconDownloader IconDownloader = new();

        private readonly IconCache _iconCache;

        public const string DefaultIconUri = "pack://application:,,,/Orc.NuGetExplorer.Xaml;component/Resources/Images/default-package-icon.png";

        public PackageMetadataMediaDownloadService(IApplicationCacheProvider appCacheProvider)
        {
            ArgumentNullException.ThrowIfNull(appCacheProvider);

            _iconCache = appCacheProvider.EnsureIconCache();
            _iconCache.FallbackValue = new BitmapImage(new Uri(DefaultIconUri));
        }

        public async Task DownloadMediaForMetadataAsync(IPackageSearchMetadata packageMetadata)
        {
            ArgumentNullException.ThrowIfNull(packageMetadata);

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
            }
        }

        private async Task DownloadFromAsync(Uri uri)
        {
            if (uri is null)
            {
                //default picture
                return;
            }

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            using (var webClient = new WebClient())
            {
                var data = await IconDownloader.GetByUrlAsync(uri, webClient);
                _iconCache.SaveToCache(uri, data);
            }
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        }

        private void DownloadFrom(Uri uri)
        {
            if (uri is null)
            {
                //default picture
                return;
            }

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            using (var webClient = new WebClient())
            {
                var data = IconDownloader.GetByUrl(uri, webClient);
                _iconCache.SaveToCache(uri, data);
            }
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        }

        #region IImageResolveService

        public ImageSource ResolveImageFromUri(Uri uri)
        {
            try
            {
                if (uri is not null && Uri.IsWellFormedUriString(uri.ToString(), UriKind.RelativeOrAbsolute))
                {
                    var bitmapImage = GetFromCacheOrFetch(uri);
                    if (bitmapImage is not null)
                    {
                        return bitmapImage;
                    }
                }
            }
            catch (WebException ex)
            {
                Log.Error(ex);
            }
            return new BitmapImage(new Uri(DefaultIconUri));
        }

        private ImageSource? GetFromCacheOrFetch(Uri uri)
        {
            if (!_iconCache.IsCached(uri))
            {
                DownloadFrom(uri);
            }

            return _iconCache.GetFromCache(uri);
        }

        public async Task<ImageSource?> ResolveImageFromUriAsync(Uri uri)
        {
            try
            {
                if (uri is not null && Uri.IsWellFormedUriString(uri.ToString(), UriKind.RelativeOrAbsolute))
                {
                    return await GetFromCacheOrFetchAsync(uri);
                }
            }
            catch (WebException ex)
            {
                Log.Error(ex);
            }

            return new BitmapImage(new Uri(DefaultIconUri));
        }

        private async Task<ImageSource?> GetFromCacheOrFetchAsync(Uri uri)
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
