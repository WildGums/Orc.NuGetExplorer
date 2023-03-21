namespace Orc.NuGetExplorer.Cache;

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

    private readonly CacheStorage<string, byte[]> _cache = new();


    public IconCache(ExpirationPolicy? cacheItemPolicy = null)
    {
        StoringPolicy = cacheItemPolicy ?? DefaultStoringPolicy;
    }

    public BitmapImage? FallbackValue { get; set; }

    public ExpirationPolicy StoringPolicy { get; }

    public void SaveToCache(Uri iconUri, byte[] streamContent)
    {
        ArgumentNullException.ThrowIfNull(iconUri);
        ArgumentNullException.ThrowIfNull(streamContent);

        _cache.Add(iconUri.ToString(), streamContent, StoringPolicy);
    }

    // TODO stream should be disposed when item removed from cache
    public BitmapImage? GetFromCache(Uri? iconUri)
    {
        try
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

            using var stream = new MemoryStream(cachedItem);
            return CreateImage(stream);
        }
        catch (Exception)
        {
            return FallbackValue;
        }
    }

    public bool IsCached(Uri iconUri)
    {
        ArgumentNullException.ThrowIfNull(iconUri);

        var cachedItem = _cache.Get(iconUri.ToString());

        return cachedItem is not null;
    }

    private BitmapImage CreateImage(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

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
        ArgumentNullException.ThrowIfNull(uri);

        var image = new BitmapImage();

        // Find extracted resource in folder from uri
        var iconUri = uri.GetLocalUriForFragment();

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
