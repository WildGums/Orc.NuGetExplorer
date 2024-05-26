namespace Orc.NuGetExplorer;

using System;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;

public interface IPackageOperationService
{
    Task UninstallPackageAsync(IPackageDetails package, Func<PackageIdentity, bool>? packagePredicate = null, CancellationToken token = default);
    Task InstallPackageAsync(IPackageDetails package, bool allowedPrerelease = false, Func<PackageIdentity, bool>? packagePredicate = null, CancellationToken token = default);
    Task UpdatePackagesAsync(IPackageDetails package, bool allowedPrerelease = false, Func<PackageIdentity, bool>? packagePredicate = null, CancellationToken token = default);
}
