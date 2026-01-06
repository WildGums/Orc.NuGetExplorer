namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;

public interface IPackagesUpdatesSearcherService
{
    Task<IReadOnlyList<IPackageDetails>> SearchForUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default);
    Task<IReadOnlyList<IPackageSearchMetadata>> SearchForPackagesUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default);
    Task<IReadOnlyList<IPackageDetails>> SearchForUpdatesAsync(string[] excludeReleasesTag, bool? allowPrerelease = null, CancellationToken token = default);
}
