// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageCacheService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Services
{
    using NuGet;
    using Orc.NuGetExplorer.Models;

    public interface IPackageCacheService
    {
        PackageDetails GetPackageDetails(IPackage package);
    }
}