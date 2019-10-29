namespace Orc.NuGetExplorer.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;

    public interface IPackageInstallationService
    {
        Task InstallAsync(PackageIdentity package, IEnumerable<IExtensibleProject> projects, CancellationToken cancellationToken);
        Task<IDictionary<SourcePackageDependencyInfo, DownloadResourceResult>> InstallAsync(
            PackageIdentity package,
            IExtensibleProject project,
            IReadOnlyList<SourceRepository> repositories,
            CancellationToken cancellationToken);
        Task UninstallAsync(PackageIdentity package, IEnumerable<IExtensibleProject> projects, CancellationToken cancellationToken);
        Task UninstallAsync(PackageIdentity package, IExtensibleProject project, CancellationToken cancellationToken);
    }
}
