namespace Orc.NuGetExplorer.Cache
{
    using NuGet.Protocol.Core.Types;
    public interface INuGetCacheManager
    {
        bool ClearAll();

        SourceCacheContext GetCacheContext();

        HttpSourceCacheContext GetHttpCacheContext();

        HttpSourceCacheContext GetHttpCacheContext(int retryCount, bool directDownload = false);
    }
}
