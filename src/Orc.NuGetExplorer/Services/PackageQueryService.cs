// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageQueryService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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
        private readonly IRepositoryCacheService _repositoryCacheService;
        #endregion

        #region Constructors
        public PackageQueryService(IPackageCacheService packageCacheService, IRepositoryCacheService repositoryCacheService)
        {
            Argument.IsNotNull(() => packageCacheService);
            Argument.IsNotNull(() => repositoryCacheService);

            _packageCacheService = packageCacheService;
            _repositoryCacheService = repositoryCacheService;
        }
        #endregion

        #region Methods
        public int CountPackages(IRepository packageRepository, IPackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageRepository);

            var count = _repositoryCacheService.GetNuGetRepository(packageRepository).GetPackages().Count(x => string.Equals(x.GetFullName(), packageDetails.FullName));
            return count;
        }

        public IPackageDetails GetPackage(IRepository packageRepository, string packageId, string version)
        {
            Argument.IsNotNull(() => packageRepository);

            var nuGetRepository = _repositoryCacheService.GetNuGetRepository(packageRepository);
            var semanticVersion = SemanticVersion.Parse(version);
            var package = nuGetRepository.FindPackage(packageId, semanticVersion);
            if (package != null)
            {
                return _packageCacheService.GetPackageDetails(nuGetRepository, package, !string.IsNullOrWhiteSpace(semanticVersion.SpecialVersion));
            }

            return null;
        }

        public int CountPackages(IRepository packageRepository, string packageId)
        {
            Argument.IsNotNull(() => packageRepository);

            var count = _repositoryCacheService.GetNuGetRepository(packageRepository).GetPackages().Count(x => string.Equals(x.Id, packageId));
            return count;
        }

        [Time]
        public int CountPackages(IRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            Argument.IsNotNull(() => packageRepository);

            var nuGetRepository = _repositoryCacheService.GetNuGetRepository(packageRepository);

            return CountPackages(nuGetRepository, filter, allowPrereleaseVersions);
        }

        private int CountPackages(IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            Argument.IsNotNull(() => packageRepository);

            try
            {
                switch (packageRepository)
                {
                    case AggregateRepository aggregateRepository:
                        return aggregateRepository.Repositories.Select(x => CountPackages(x, filter, allowPrereleaseVersions)).Sum();

                    default:
                    {
                        var queryable = packageRepository.Search(filter, allowPrereleaseVersions)
                            .Where(x => x.IsAbsoluteLatestVersion || x.IsLatestVersion);

                        return queryable.Count();
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public IEnumerable<IPackageDetails> GetPackages(IRepository packageRepository, bool allowPrereleaseVersions,
            string filter = null, int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            try
            {
                Log.Debug("Getting {0} packages starting from {1}, which contains \"{2}\"", take, skip, filter);
                var nuGetRepository = _repositoryCacheService.GetNuGetRepository(packageRepository);

                return nuGetRepository.FindFiltered(filter, allowPrereleaseVersions, skip, take).Select(package => _packageCacheService.GetPackageDetails(nuGetRepository, package, allowPrereleaseVersions));
            }
            catch (Exception exception)
            {
                Log.Warning(exception);

                return Enumerable.Empty<PackageDetails>();
            }
        }

        public IEnumerable<IPackageDetails> GetVersionsOfPackage(IRepository packageRepository, IPackageDetails package, bool allowPrereleaseVersions, ref int skip, int minimalTake = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            try
            {
                var nuGetRepository = _repositoryCacheService.GetNuGetRepository(packageRepository);

                return nuGetRepository.FindPackageVersions(package.ToNuGetPackage(), allowPrereleaseVersions, ref skip, minimalTake)
                    .Select(p => _packageCacheService.GetPackageDetails(nuGetRepository, p, allowPrereleaseVersions));
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
