namespace Orc.NuGetExplorer;

using System;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;

//Helper for v2 NuGet - eager loading for packages versions from v2, because they failed later with NRE, since
//lazyFactory inside ClonePackageSearchMetadata contains reference on CancellationToken used in SearchAsync
public static class V2SearchHelper
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(V2SearchHelper));

    public static async Task GetVersionsMetadataAsync(IPackageSearchMetadata package)
    {
        ArgumentNullException.ThrowIfNull(package);

        try
        {
            await package.GetVersionsAsync();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Cannot preload metadata for package {PackageId} of version {PackageVersion} from v2 feed due to error", package.Identity.Id, package.Identity.Version);
        }
    }
}
