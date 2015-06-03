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
    using Catel.Threading;

    public static class IPackageQueryServiceExtensions
    {
        #region Methods
        public static Task<IEnumerable<IPackageDetails>> GetPackagesAsync(this IPackageQueryService packageQueryService,
            IRepository packageRepository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageQueryService);

            return TaskHelper.Run(() => packageQueryService.GetPackages(packageRepository, allowPrereleaseVersions, filter, skip, take));
        }

        public static Task<IEnumerable<IPackageDetails>> GetVersionsOfPackageAsync(this IPackageQueryService packageQueryService, IRepository packageRepository, IPackageDetails package, bool allowPrereleaseVersions, int skip, int minimalTake = 10)
        {
            Argument.IsNotNull(() => packageQueryService);

            return TaskHelper.Run(() => packageQueryService.GetVersionsOfPackage(packageRepository, package, allowPrereleaseVersions, ref skip, minimalTake));
        }

        public static Task<int> CountPackagesAsync(this IPackageQueryService packageQueryService, IRepository packageRepository,
            string filter, bool allowPrereleaseVersions)
        {
            Argument.IsNotNull(() => packageQueryService);

            return TaskHelper.Run(() => packageQueryService.CountPackages(packageRepository, filter, allowPrereleaseVersions));
        }
        #endregion
    }
}