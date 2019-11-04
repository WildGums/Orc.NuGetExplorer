namespace Orc.NuGetExplorer.Cache
{
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Catel.Caching;
    using Catel.Caching.Policies;

    public class IconCache
    {
        private static readonly ExpirationPolicy DefaultStoringPolicy = ExpirationPolicy.Duration(TimeSpan.FromDays(30));

        private readonly CacheStorage<string, byte[]> _cache = new CacheStorage<string, byte[]>();


        public IconCache(ExpirationPolicy cacheItemPolicy = null)
        {
            StoringPolicy = cacheItemPolicy ?? DefaultStoringPolicy;
        }

        public BitmapImage FallbackValue { get; set; }

        public ExpirationPolicy StoringPolicy { get; private set; }

        public void SaveToCache(Uri iconUri, byte[] streamContent)
        {
            _cache.Add(iconUri.ToString(), streamContent, StoringPolicy);
        }

        public BitmapImage GetFromCache(Uri iconUri)
        {
            //todo stream should be disposed when item removed from cache
            if (iconUri == null)
            {
                return FallbackValue;
            }

            string key = iconUri.ToString();
            var cachedItem = _cache.Get(key);

            if (cachedItem == null)
            {
                return FallbackValue;
            }

            using (var stream = new MemoryStream(cachedItem))
            {
                var image = new BitmapImage();

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();

                return image;
            }
        }

        public bool IsCached(Uri iconUri)
        {
            if (iconUri == null)
            {
                return false;
            }

            var cachedItem = _cache.Get(iconUri.ToString());

            return cachedItem != null;
        }
    }
}
