namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Protocol.Core.Types;

    public interface IDownloadingProgressTrackerService
    {
        Task<IDisposableToken<IProgress<float>>> TrackDownloadOperationAsync(IPackageInstallationService packageInstallationService, SourcePackageDependencyInfo packageDependencyInfo);
    }
}
