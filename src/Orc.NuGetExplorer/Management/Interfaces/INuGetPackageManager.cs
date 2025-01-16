namespace Orc.NuGetExplorer.Management;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Catel;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Orc.NuGetExplorer.Packaging;

public interface INuGetPackageManager : IPackageManager
{
    Task<bool> InstallPackageForProjectAsync(PackageInstallationContext context, CancellationToken token);

    [ObsoleteEx(ReplacementTypeOrMember = "InstallPackageForProjectAsync(PackageInstallationContext context, CancellationToken token)", TreatAsErrorFromVersion = "6", RemoveInVersion = "7")]
    Task<bool> InstallPackageForProjectAsync(IExtensibleProject project, PackageIdentity package,
        Func<PackageIdentity, bool>? packagePredicate, CancellationToken token, bool showErrors = true);

    Task UninstallPackageForProjectAsync(IExtensibleProject project, PackageIdentity package,
        Func<PackageIdentity, bool>? packagePredicate, CancellationToken token);

    Task UpdatePackageForProjectAsync(IExtensibleProject project, string packageId, NuGetVersion targetVersion,
        Func<PackageIdentity, bool>? packagePredicate, CancellationToken token);

    Task<IEnumerable<PackageReference>> GetInstalledPackagesAsync(IExtensibleProject project, CancellationToken token);

    Task<bool> IsPackageInstalledAsync(IExtensibleProject project, PackageIdentity package, CancellationToken token);
    Task<bool> IsPackageInstalledAsync(IExtensibleProject project, string packageId, CancellationToken token);

    Task<PackageCollection> CreatePackagesCollectionFromProjectsAsync(IEnumerable<IExtensibleProject> projects, CancellationToken cancellationToken);

    IEnumerable<SourceRepository> AsLocalRepositories(IEnumerable<IExtensibleProject> projects);
    Task<NuGetVersion?> GetVersionInstalledAsync(IExtensibleProject project, string packageId, CancellationToken token);

    event AsyncEventHandler<InstallNuGetProjectEventArgs>? Install;

    event AsyncEventHandler<UninstallNuGetProjectEventArgs>? Uninstall;

    event AsyncEventHandler<UpdateNuGetProjectEventArgs>? Update;
}
