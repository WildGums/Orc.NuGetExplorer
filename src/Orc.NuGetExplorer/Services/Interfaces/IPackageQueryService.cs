// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageQueryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using NuGet;

    public interface IPackageQueryService
    {
        #region Methods
        IEnumerable<PackageDetails> GetPackages(IEnumerable<PackageSource> packageSources, string filter = null, int skip = 0, int take = 10);
        IEnumerable<PackageDetails> GetPackages(IPackageRepository packageRepository, string filter = null, int skip = 0, int take = 10);
        int GetPackagesCount(IPackageRepository packageRepository, string filter);
        #endregion
    }
}