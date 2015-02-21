// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageQueryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Versioning;
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

            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(packageSources.First().Source);

            return GetPackages(repo, filter, skip, take);
        }

        public IEnumerable<PackageDetails> GetPackages(IPackageRepository packageRepository, string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            var queryable = packageRepository.GetPackages();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.Trim();
                queryable = queryable.Where(x => x.Title.Contains(filter)).OrderBy(x => x.DownloadCount);
            }

            
            queryable = queryable.Where(x => x.IsLatestVersion);
            // TODO: Merge results with multiple package sources
            return queryable.Skip(skip).Take(take).ToList().Select(x => _packageCacheService.GetPackageDetails(x));
        }
        #endregion
    }
}