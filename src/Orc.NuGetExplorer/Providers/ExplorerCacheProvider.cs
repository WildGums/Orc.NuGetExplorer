namespace Orc.NuGetExplorer.Providers
{
    using NuGetExplorer.Cache;

    public class ExplorerCacheProvider : IApplicationCacheProvider
    {
        private IconCache _iconCache;

        public IconCache EnsureIconCache()
        {
            if (_iconCache is null)
            {
                _iconCache = new IconCache();
            }

            return _iconCache;
        }
    }
}
