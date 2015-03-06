// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageCacheService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal interface IPackageCacheService
    {
        #region Methods
        PackageDetails GetPackageDetails(IPackage package);
        #endregion
    }
}