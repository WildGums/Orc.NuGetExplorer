using Orc.NuGetExplorer.Cache;

namespace Orc.NuGetExplorer.Providers
{
    using NuGetExplorer.Cache;

    public class ApplcationCacheProvider : IApplicationCacheProvider
    {
        IconCache iconCache;

        public IconCache EnsureIconCache()
        {
            if (iconCache == null)
            {
                iconCache = new IconCache();
            }

            return iconCache;
        }
    }
}
