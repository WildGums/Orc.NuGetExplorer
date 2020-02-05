namespace Orc.NuGetExplorer.Providers
{
    using NuGetExplorer.Cache;

    public class ExplorerCacheProvider : IApplicationCacheProvider
    {
        private IconCache _iconCache;

        public IconCache EnsureIconCache()
        {
            if (_iconCache == null)
            {
                _iconCache = new IconCache();
            }

            return _iconCache;
        }
    }
}
