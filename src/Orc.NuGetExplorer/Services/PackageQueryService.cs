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
        public IEnumerable<PackageDetails> GetPackages(string packageSource, string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNullOrWhitespace(() => packageSource);

            var packageSources = new List<string>();

            if (packageSource.IsAllSource())
            {
                packageSources.AddRange(_packageSourceService.PackageSources.Select(x => x.Name));
            }
            else
            {
                packageSources.Add(packageSource);
            }

            return GetPackages(packageSources.ToArray(), filter, skip, take);
        }

        public IEnumerable<PackageDetails> GetPackages(string[] packageSources, string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageSources);

            foreach (var packageSource in packageSources)
            {
                var finalSource = _packageSourceService.PackageSources.Where(x => string.Equals(x.Name, packageSource, StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Source).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(finalSource))
                {
                    var packageRepository = PackageRepositoryFactory.Default.CreateRepository(finalSource);

                    var queryable = packageRepository.GetPackages();
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        filter = filter.Trim();
                        queryable = queryable.Where(x => x.Title.Contains(filter));
                    }

                    queryable = queryable.Where(x => x.IsLatestVersion);

                    // TODO: Merge results with multiple package sources
                    return queryable.Skip(skip).Take(take).ToList().Select(x => _packageCacheService.GetPackageDetails(x));
                }
            }

            return new PackageDetails[] {};
        }
        #endregion
    }
}