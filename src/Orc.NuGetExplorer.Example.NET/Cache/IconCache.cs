namespace Orc.NuGetExplorer.Cache
{
    using Catel.Caching;
    using Catel.Caching.Policies;
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;

    public class IconCache
    {
        private readonly CacheStorage<string, byte[]> Cache = new CacheStorage<string, byte[]>();

        private static readonly ExpirationPolicy DefaultStoringPolicy = ExpirationPolicy.Duration(TimeSpan.FromDays(30));

        public BitmapImage FallbackValue { get; set; }

        public IconCache(ExpirationPolicy cacheItemPolicy = null)
        {
            StoringPolicy = cacheItemPolicy ?? DefaultStoringPolicy;
        }


        public ExpirationPolicy StoringPolicy { get; private set; }

        public void SaveToCache(Uri iconUri, byte[] streamContent)
        {
            Cache.Add(iconUri.ToString(), streamContent, StoringPolicy);
        }

        public BitmapImage GetFromCache(Uri iconUri)
        {
            //todo stream should be disposed when item removed from cache
            if (iconUri == null)
            {
                return FallbackValue;
            }

            string key = iconUri.ToString();
            var cachedItem = Cache.Get(key);

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
    }
}
