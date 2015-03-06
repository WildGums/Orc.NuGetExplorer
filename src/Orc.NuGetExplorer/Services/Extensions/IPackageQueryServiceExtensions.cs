// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageQueryServiceExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using NuGet;

    internal static class IPackageQueryServiceExtensions
    {
        public static async Task<IEnumerable<PackageDetails>> GetPackagesAsync(this IPackageQueryService packageQueryService,
            IPackageRepository packageRepository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageQueryService);

            return await Task.Factory.StartNew(() => packageQueryService.GetPackages(packageRepository, allowPrereleaseVersions, filter, skip, take));
        }

        public static async Task<IEnumerable<PackageDetails>> GetVersionsOfPackageAsync(this IPackageQueryService packageQueryService, IPackageRepository packageRepository, IPackage package, bool allowPrereleaseVersions, int skip, int minimalTake = 10)
        {
            Argument.IsNotNull(() => packageQueryService);

            return await Task.Factory.StartNew(() => packageQueryService.GetVersionsOfPackage(packageRepository, package, allowPrereleaseVersions, ref skip, minimalTake));
        }
    }
}