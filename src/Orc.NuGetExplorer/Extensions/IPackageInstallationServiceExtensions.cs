namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MethodTimer;
using Models;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Services;

public static class IPackageInstallationServiceExtensions
{
    [Time]
    public static async Task<InstallerResult> InstallAsync(
        this IPackageInstallationService service,
        PackageIdentity package,
        IExtensibleProject project,
        IReadOnlyList<SourceRepository> repositories,
        bool ignoreMissingPackages = false,
        Func<PackageIdentity, bool>? packagePredicate = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(repositories);

        var context = new InstallationContext
        {
            Package = package,
            Project = project,
            Repositories = repositories,
            IgnoreMissingPackages = ignoreMissingPackages,
            PackagePredicate = packagePredicate,
        };

        return await service.InstallAsync(context, cancellationToken);
    }

}
