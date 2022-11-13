namespace Orc.NuGetExplorer
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPackageOperationService
    {
        Task UninstallPackageAsync(IPackageDetails package, CancellationToken token = default);
        Task InstallPackageAsync(IPackageDetails package, bool allowedPrerelease = false, CancellationToken token = default);
        Task UpdatePackagesAsync(IPackageDetails package, bool allowedPrerelease = false, CancellationToken token = default);
    }
}
