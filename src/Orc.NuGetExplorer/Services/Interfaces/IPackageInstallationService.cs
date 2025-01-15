namespace Orc.NuGetExplorer.Services;

using System;
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

    Task<InstallerResult> InstallAsync(InstallationContext context, CancellationToken cancellationToken = default);

    [ObsoleteEx(ReplacementTypeOrMember = "InstallAsync(InstallationContext context, CancellationToken cancellationToken = default)", TreatAsErrorFromVersion = "6", RemoveInVersion = "7")]
    public Task<InstallerResult> InstallAsync(
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
