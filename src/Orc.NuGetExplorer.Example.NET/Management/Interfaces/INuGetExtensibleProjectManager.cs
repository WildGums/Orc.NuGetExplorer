using Catel;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Orc.NuGetExplorer.Management.EventArgs;

namespace Orc.NuGetExplorer.Management
{
    public interface INuGetExtensibleProjectManager
    {
        Task InstallPackageForProject(IExtensibleProject project, PackageIdentity package, CancellationToken token);

        Task UninstallPackageForProject(IExtensibleProject project, PackageIdentity package, CancellationToken token);
        Task<IEnumerable<PackageReference>> GetInstalledPackagesAsync(IExtensibleProject project, CancellationToken token);
        Task<bool> IsPackageInstalledAsync(IExtensibleProject project, PackageIdentity package, CancellationToken token);
        Task<Packaging.PackageCollection> CreatePackagesCollectionFromProjectsAsync(IEnumerable<IExtensibleProject> projects, CancellationToken cancellationToken);
        IEnumerable<NuGet.Protocol.Core.Types.SourceRepository> AsLocalRepositories(IEnumerable<IExtensibleProject> projects);
        Task UninstallPackageForMultipleProject(IReadOnlyList<IExtensibleProject> projects, PackageIdentity package, CancellationToken token);
        Task InstallPackageForMultipleProject(IReadOnlyList<IExtensibleProject> projects, PackageIdentity package, CancellationToken token);
        Task UpdatePackageForMultipleProject(IReadOnlyList<IExtensibleProject> projects, string packageid, NuGet.Versioning.NuGetVersion targetVersion, CancellationToken token);

        event AsyncEventHandler<InstallNuGetProjectEventArgs> Install;

        event AsyncEventHandler<UninstallNuGetProjectEventArgs> Uninstall;

        event AsyncEventHandler<UpdateNuGetProjectEventArgs> Update;
    }
}
