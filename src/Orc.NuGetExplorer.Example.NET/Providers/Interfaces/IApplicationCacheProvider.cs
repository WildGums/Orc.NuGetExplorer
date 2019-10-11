using Orc.NuGetExplorer.Cache;

namespace Orc.NuGetExplorer.Providers
{
    using NuGetExplorer.Cache;

    public interface IApplicationCacheProvider
    {
        IconCache EnsureIconCache();
    }
}
