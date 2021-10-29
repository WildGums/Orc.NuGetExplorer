namespace Orc.NuGetExplorer.Cache
{
    public interface INuGetCacheManager
    {
        bool ClearAll();
        bool ClearHttpCache();
    }
}
