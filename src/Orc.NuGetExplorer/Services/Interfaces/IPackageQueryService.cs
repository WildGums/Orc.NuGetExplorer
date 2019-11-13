// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageQueryService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NuGet.Protocol.Core.Types;

    public interface IPackageQueryService
    {
        #region Methods
        Task<bool> PackageExists(IRepository packageRepository, string filter, bool allowPrereleaseVersions);
        Task<bool> PackageExists(IRepository packageRepository, string packageId);
        Task<bool> PackageExists(IRepository packageRepository, IPackageDetails packageDetails);

        Task<IPackageDetails> GetPackage(IRepository packageRepository, string packageId, string version);
        Task<IEnumerable<IPackageDetails>> GetPackagesAsync(IRepository packageRepository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10);
        Task<IEnumerable<IPackageSearchMetadata>> GetVersionsOfPackage(IRepository packageRepository, IPackageDetails package, bool allowPrereleaseVersions, int skip);
        #endregion

    }
}
