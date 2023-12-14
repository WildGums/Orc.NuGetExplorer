namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;

public interface IPackagesUpdatesSearcherService
{
    Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default);
    Task<IEnumerable<IPackageSearchMetadata>> SearchForPackagesUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default);
    Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(string[] excludeReleasesTag, bool? allowPrerelease = null, CancellationToken token = default);
}