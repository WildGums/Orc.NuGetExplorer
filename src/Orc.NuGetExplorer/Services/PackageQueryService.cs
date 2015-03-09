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
        public IEnumerable<PackageDetails> GetPackages(IPackageRepository packageRepository, bool allowPrereleaseVersions,
            string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            try
            {
                Log.Info("Getting packages filter=\"{0}\", skip {1}, take {2}", filter, skip, take);

                return packageRepository.FindFiltered(filter, allowPrereleaseVersions, skip, take)
                    .Select(package => _packageCacheService.GetPackageDetails(package));
            }
            catch(Exception exception)
            {
                Log.Warning(exception);

                return Enumerable.Empty<PackageDetails>();
            }
        }

        public IEnumerable<PackageDetails> GetVersionsOfPackage(IPackageRepository packageRepository, IPackage package, bool allowPrereleaseVersions,
            ref int skip, int minimalTake = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            return packageRepository.FindPackageVersions(package, allowPrereleaseVersions, ref skip, minimalTake)
                .Select(p => _packageCacheService.GetPackageDetails(p));
        }
        #endregion
    }
}