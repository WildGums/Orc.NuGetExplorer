namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Catel;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Cache;
    using Orc.NuGetExplorer.Providers;

    public class PackageMetadataMediaDownloadService : IPackageMetadataMediaDownloadService, IImageResolveService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IconCache _iconCache;

        private readonly string _defaultIconUri = "pack://application:,,,/Orc.NuGetExplorer.Xaml;component/Resources/Images/default-package-icon.png";

        private static readonly HttpClient HttpClient = new();

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
            }
        }

        private async Task DownloadFromAsync(Uri uri)
        {
            if (uri is null)
            {
                return;
            }

            Log.Debug($"Request media content from uri {uri}");

            using (var response = await HttpClient.GetAsync(uri))
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                _iconCache.SaveToCache(uri, data);
            }
        }

        #region IImageResolveService

        public async Task<ImageSource> ResolveImageFromUriAsync(Uri uri, string defaultUrl = null)
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
