// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageQueryService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackageQueryService
    {
        #region Methods
        IEnumerable<IPackageDetails> GetPackages(IRepository packageRepository, bool allowPrereleaseVersions,
            string filter = null, int skip = 0, int take = 10);

        IEnumerable<IPackageDetails> GetVersionsOfPackage(IRepository packageRepository, IPackageDetails package, bool allowPrereleaseVersions,
            ref int skip, int minimalTake = 10);

        IPackageDetails GetPackage(IRepository packageRepository, string packageId, string version);

        int CountPackages(IRepository packageRepository, string filter, bool allowPrereleaseVersions);
        int CountPackages(IRepository packageRepository, string packageId);
        int CountPackages(IRepository packageRepository, IPackageDetails packageDetails);
        #endregion

    }
}