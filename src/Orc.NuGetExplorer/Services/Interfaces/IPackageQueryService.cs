namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NuGet.Protocol.Core.Types;

    public interface IPackageQueryService
    {
        #region Methods
        Task<bool> PackageExistsAsync(IRepository packageRepository, string filter, bool allowPrereleaseVersions);
        Task<bool> PackageExistsAsync(IRepository packageRepository, string packageId);
        Task<bool> PackageExistsAsync(IRepository packageRepository, IPackageDetails packageDetails);

        Task<IPackageDetails> GetPackageAsync(IRepository packageRepository, string packageId, string version);
        Task<IEnumerable<IPackageDetails>> GetPackagesAsync(IRepository packageRepository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10);
        Task<IEnumerable<IPackageSearchMetadata>> GetVersionsOfPackageAsync(IRepository packageRepository, IPackageDetails package, bool allowPrereleaseVersions, int skip);
        #endregion

    }
}
