namespace Orc.NuGetExplorer;

using System;
using System.Threading.Tasks;
using Catel.Logging;
using NuGet.Protocol.Core.Types;

//Helper for v2 NuGet - eager loading for packages versions from v2, because they failed later with NRE, since
//lazyFactory inside ClonePackageSearchMetadata constains reference on CancellationToken used in SearchAsync
public static class V2SearchHelper
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    public static async Task GetVersionsMetadataAsync(IPackageSearchMetadata package)
    {
        ArgumentNullException.ThrowIfNull(package);

        try
        {
            await package.GetVersionsAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, $"Cannot preload metadata for package {package.Identity.Id} of version {package.Identity.Version} from v2 feed due to error");
        }
    }
}