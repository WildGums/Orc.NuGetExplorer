// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageQueryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using MethodTimer;
    using NuGet;

    internal class PackageQueryService : IPackageQueryService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IPackageCacheService _packageCacheService;
        #endregion

        #region Constructors
        public PackageQueryService(IPackageCacheService packageCacheService)
        {
            Argument.IsNotNull(() => packageCacheService);

            _packageCacheService = packageCacheService;
        }
        #endregion

        #region Methods
        public int CountPackages(IPackageRepository packageRepository, IPackageDetails packageDetails)
        {
            var count = packageRepository.GetPackages().Count(x => string.Equals(x.GetFullName(), packageDetails.FullName));
            return count;
        }

        public int CountPackages(IPackageRepository packageRepository, string packageId)
        {
            var count = packageRepository.GetPackages().Count(x => string.Equals(x.Id, packageId));
            return count;
        }

        [Time]
        public int CountPackages(IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            Argument.IsNotNull(() => packageRepository);

            try
            {
                var queryable = packageRepository.BuildQueryForSingleVersion(filter, allowPrereleaseVersions);
                var count = queryable.Count();
                return count;
            }
            catch
            {
                return 0;
            }
        }

        public IEnumerable<PackageDetails> GetPackages(IPackageRepository packageRepository, bool allowPrereleaseVersions,
            string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            try
            {
                Log.Debug("Getting {0} packages starting from {1}, which contains \"{2}\"", take, skip, filter);

                return packageRepository.FindFiltered(filter, allowPrereleaseVersions, skip, take)
                    .Select(package => _packageCacheService.GetPackageDetails(package));
            }
            catch (Exception exception)
            {
                Log.Warning(exception);

                return Enumerable.Empty<PackageDetails>();
            }
        }

        public IEnumerable<PackageDetails> GetVersionsOfPackage(IPackageRepository packageRepository, IPackage package, bool allowPrereleaseVersions,
            ref int skip, int minimalTake = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            try
            {
                return packageRepository.FindPackageVersions(package, allowPrereleaseVersions, ref skip, minimalTake)
                    .Select(p => _packageCacheService.GetPackageDetails(p));
            }
            catch (Exception exception)
            {
                Log.Warning(exception);

                return Enumerable.Empty<PackageDetails>();
            }
        }
        #endregion
    }
}