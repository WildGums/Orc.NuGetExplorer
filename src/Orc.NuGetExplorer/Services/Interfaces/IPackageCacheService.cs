// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageCacheService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    public interface IPackageCacheService
    {
        PackageDetails GetPackageDetails(IPackage package);
    }
}