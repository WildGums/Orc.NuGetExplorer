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
        private readonly IPackageSourceService _packageSourceService;
        #endregion

        #region Constructors
        public PackageQueryService(IPackageSourceService packageSourceService, IPackageCacheService packageCacheService)
        {
            Argument.IsNotNull(() => packageSourceService);
            Argument.IsNotNull(() => packageCacheService);

            _packageSourceService = packageSourceService;
            _packageCacheService = packageCacheService;
        }
        #endregion

        #region Methods
        public IEnumerable<PackageDetails> GetPackages(IEnumerable<PackageSource> packageSources, string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageSources);

            IPackageRepository repo = new AggregateRepository(PackageRepositoryFactory.Default, packageSources.Select(x => x.Source), true);

            return GetPackages(repo, filter, skip, take);
        }

        public IEnumerable<PackageDetails> GetPackages(IPackageRepository packageRepository, string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            var queryable = CreateQuery(packageRepository, filter);
            return queryable.OrderByDescending(x => x.DownloadCount).Skip(skip).Take(take).ToList().Select(x => _packageCacheService.GetPackageDetails(x));
        }

        public int GetPackagesCount(IPackageRepository packageRepository, string filter)
        {
            var queryable = CreateQuery(packageRepository, filter);
            var count = queryable.Count();
            return count;
        }

        private static IQueryable<IPackage> CreateQuery(IPackageRepository packageRepository, string filter)
        {
            var queryable = packageRepository.GetPackages();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.Trim();
                queryable = queryable.Where(x => x.Title.Contains(filter));
            }

            queryable = queryable.Where(x => x.IsLatestVersion);
            return queryable;
        }
        #endregion
    }
}