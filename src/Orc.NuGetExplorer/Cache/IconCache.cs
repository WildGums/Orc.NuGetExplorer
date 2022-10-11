namespace Orc.NuGetExplorer.Cache
{
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Catel.Caching;
    using Catel.Caching.Policies;
    using Catel.Logging;

    public class IconCache
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ExpirationPolicy DefaultStoringPolicy = ExpirationPolicy.Duration(TimeSpan.FromDays(30))!;

        private readonly CacheStorage<string, byte[]> _cache = new CacheStorage<string, byte[]>();


        public IconCache(ExpirationPolicy? cacheItemPolicy = null)
        {
            StoringPolicy = cacheItemPolicy ?? DefaultStoringPolicy;
        }

        public BitmapImage? FallbackValue { get; set; }

        public ExpirationPolicy StoringPolicy { get; private set; }

        public void SaveToCache(Uri iconUri, byte[] streamContent)
        {
            _cache.Add(iconUri.ToString(), streamContent, StoringPolicy);
        }

        // TODO stream should be disposed when item removed from cache
        public BitmapImage? GetFromCache(Uri iconUri)
        {
            if (iconUri is null)
            {
                return FallbackValue;
            }

            if (iconUri.IsLoopback)
            {
                return CreateImage(iconUri);
            }

            var cachedItem = _cache.Get(iconUri.ToString());

            if (cachedItem is null)
            {
                return FallbackValue;
            }

            using (var stream = new MemoryStream(cachedItem))
            {
                return CreateImage(stream);
            }
        }

        public bool IsCached(Uri iconUri)
        {
            if (iconUri is null)
            {
                return false;
            }

            var cachedItem = _cache.Get(iconUri.ToString());

            return cachedItem is not null;
        }

        private BitmapImage CreateImage(Stream stream)
        {
            var image = new BitmapImage();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();

            if (!image.IsFrozen)
            {
                image.Freeze();
            }

            return image;
        }

        private BitmapImage CreateImage(Uri uri)
        {
            var image = new BitmapImage();

            // Find extracted resource in folder from uri
            var iconUri = uri;

            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                var fileName = uri.Fragment[1..];
                var folderPath = Path.GetDirectoryName(uri.AbsolutePath);
                if (string.IsNullOrEmpty(folderPath))
                {
                    Log.Debug("Cannot parse source uri for local icon");
                }
                else
                {
                    // Decode spaces
                    folderPath = folderPath.Replace("%20", " ");
                    iconUri = new Uri(Path.Combine(folderPath, fileName));
                }
            }

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = iconUri;
            image.EndInit();

            if (!image.IsFrozen)
            {
                image.Freeze();
            }

            return image;
        }
    }
}
