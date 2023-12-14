namespace Orc.NuGetExplorer.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

public interface IPackageInstallationService
{
    /// <summary>
    /// V3 package path resolver
    /// </summary>
    VersionFolderPathResolver InstallerPathResolver { get; }

    Task<InstallerResult> InstallAsync(
        PackageIdentity package,
        IExtensibleProject project,
        IReadOnlyList<SourceRepository> repositories,
        bool ignoreMissingPackages = false,
        CancellationToken cancellationToken = default);

    Task UninstallAsync(PackageIdentity package, IExtensibleProject project, IEnumerable<PackageReference> installedPackageReferences,
        CancellationToken cancellationToken = default);

    Task<long?> MeasurePackageSizeFromRepositoryAsync(PackageIdentity packageIdentity, SourceRepository sourceRepository);
}