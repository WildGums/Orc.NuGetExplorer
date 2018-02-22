// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageCacheService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal interface IPackageCacheService
    {
        #region Methods
        PackageDetails GetPackageDetails(IPackageRepository packageRepository, IPackage package, bool allowPrereleaseVersions);
        #endregion
    }
}