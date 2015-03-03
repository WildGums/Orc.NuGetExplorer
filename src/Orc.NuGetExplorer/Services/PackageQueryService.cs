// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageQueryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using NuGet;

    public class PackageQueryService : IPackageQueryService
    {
        #region Fields
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

            var queryable = CreateQuery(packageRepository, filter, allowPrereleaseVersions);
            var packages = queryable.OrderByDescending(x => x.DownloadCount).Skip(skip).Take(take).ToList();
            return packages.Select(x => _packageCacheService.GetPackageDetails(x));
        }

        public int GetPackagesCount(IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            var queryable = CreateQuery(packageRepository, filter, allowPrereleaseVersions);
            var count = queryable.Count();
            return count;
        }

        private static IQueryable<IPackage> CreateQuery(IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            var queryable = packageRepository.GetPackages();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.Trim();
                queryable = queryable.Where(x => x.Title.Contains(filter));
            }

            if (allowPrereleaseVersions)
            {
                queryable = queryable.Where(x => x.IsAbsoluteLatestVersion);
            }
            else
            {
                queryable = queryable.Where(x => x.IsLatestVersion);
            }
            
            return queryable;
        }
        #endregion
    }
}