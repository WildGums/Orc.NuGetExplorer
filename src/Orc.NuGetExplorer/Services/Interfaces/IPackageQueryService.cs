// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageQueryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using NuGet;

    internal interface IPackageQueryService
    {
        #region Methods
        IEnumerable<PackageDetails> GetPackages(IPackageRepository packageRepository, bool allowPrereleaseVersions,
            string filter = null, int skip = 0, int take = 10);

        IEnumerable<PackageDetails> GetVersionsOfPackage(IPackageRepository packageRepository, IPackage package, bool allowPrereleaseVersions,
            ref int skip, int minimalTake = 10);

        int CountPackages(IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions);

        int CountPackages(IPackageRepository packageRepository, string packageId);
        #endregion
    }
}