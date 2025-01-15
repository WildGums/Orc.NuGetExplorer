namespace Orc.NuGetExplorer.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orc.NuGetExplorer.Models;

public interface IPackageInstallationService
{
    /// <summary>
    /// V3 package path resolver
    /// </summary>
    VersionFolderPathResolver InstallerPathResolver { get; }

    Task<InstallerResult> InstallAsync(InstallationContext context);

    Task<InstallerResult> InstallAsync(
        PackageIdentity package,
        IExtensibleProject project,
        IReadOnlyList<SourceRepository> repositories,
        bool ignoreMissingPackages = false,
        Func<PackageIdentity, bool>? packagePredicate = null,
        CancellationToken cancellationToken = default);

    Task UninstallAsync(PackageIdentity package, 
        IExtensibleProject project, 
        IEnumerable<PackageReference> installedPackageReferences,
        Func<PackageIdentity, bool>? packagePredicate = null,
        CancellationToken cancellationToken = default);

    Task<long?> MeasurePackageSizeFromRepositoryAsync(PackageIdentity packageIdentity, SourceRepository sourceRepository);
}
