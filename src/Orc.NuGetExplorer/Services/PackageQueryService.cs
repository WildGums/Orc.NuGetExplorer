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

            var queryable = CreateQueryForSingleVersion(packageRepository, filter, allowPrereleaseVersions);
            var packages = queryable.OrderByDescending(x => x.DownloadCount).Skip(skip).Take(take).ToList();
            return packages.Select(x => _packageCacheService.GetPackageDetails(x));
        }

        public int GetPackagesCount(IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            var queryable = CreateQueryForSingleVersion(packageRepository, filter, allowPrereleaseVersions);
            var count = queryable.Count();
            return count;
        }

        public IEnumerable<IPackage> GetVersionsOfPackage(IPackageRepository packageRepository, IPackage package, bool allowPrereleaseVersions,
            ref int skip, int minimalTake = 10)
        {
            if (skip < 0)
            {
                return Enumerable.Empty<IPackage>();
            }

            var queryable = packageRepository.GetPackages().Where(x => Equals(x.Id, package.Id)).Skip(skip).Take(minimalTake);

            var result = new List<IPackage>(queryable.ToList());

            if (result.Count < minimalTake)
            {
                skip = -1;
            }
            else
            {
                skip += minimalTake;
            }
            
            if (!allowPrereleaseVersions && result.Any())
            {
                result = result.Where(x => x.IsReleaseVersion()).ToList();

                var count = result.Count;
                
                if (skip >= 0 && count < minimalTake)
                {
                    var additional = GetVersionsOfPackage(packageRepository, package, false, ref skip, minimalTake).ToList();
                    result.AddRange(additional);
                }
            }


            return result;
        }

        private static IQueryable<IPackage> CreateQueryForSingleVersion(IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions)
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