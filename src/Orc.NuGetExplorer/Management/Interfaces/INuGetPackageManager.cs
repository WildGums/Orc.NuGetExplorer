﻿namespace Orc.NuGetExplorer.Management
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using Orc.NuGetExplorer.Management.EventArgs;
    using Orc.NuGetExplorer.Packaging;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;

    internal interface INuGetPackageManager : IPackageManager
    {
        Task InstallPackageForProjectAsync(IExtensibleProject project, PackageIdentity package, CancellationToken token);

        Task UninstallPackageForProjectAsync(IExtensibleProject project, PackageIdentity package, CancellationToken token);

        Task UpdatePackageForProjectAsync(IExtensibleProject project, string packageid, NuGetVersion targetVersion, CancellationToken token);

        Task<IEnumerable<PackageReference>> GetInstalledPackagesAsync(IExtensibleProject project, CancellationToken token);

        Task<bool> IsPackageInstalledAsync(IExtensibleProject project, PackageIdentity package, CancellationToken token);

        Task<PackageCollection> CreatePackagesCollectionFromProjectsAsync(IEnumerable<IExtensibleProject> projects, CancellationToken cancellationToken);

        IEnumerable<SourceRepository> AsLocalRepositories(IEnumerable<IExtensibleProject> projects);

        /*
        Task UninstallPackageForMultipleProject(IReadOnlyList<IExtensibleProject> projects, PackageIdentity package, CancellationToken token);
        Task InstallPackageForMultipleProject(IReadOnlyList<IExtensibleProject> projects, PackageIdentity package, CancellationToken token);
        Task UpdatePackageForMultipleProject(IReadOnlyList<IExtensibleProject> projects, string packageid, NuGet.Versioning.NuGetVersion targetVersion, CancellationToken token);
        */

        event AsyncEventHandler<InstallNuGetProjectEventArgs> Install;

        event AsyncEventHandler<UninstallNuGetProjectEventArgs> Uninstall;

        event AsyncEventHandler<UpdateNuGetProjectEventArgs> Update;
    }
}
