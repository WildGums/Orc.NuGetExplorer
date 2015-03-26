// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesUpdatesSearcherService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using NuGet;

    internal class PackagesUpdatesSearcherService : IPackagesUpdatesSearcherService
    {
        #region Fields
        private readonly IAuthenticationSilencerService _authenticationSilencerService;
        private readonly IPackageCacheService _packageCacheService;
        private readonly IRepositoryService _repositoryService;
        private readonly IRepositoryCacheService _repositoryCacheService;
        #endregion

        #region Constructors
        public PackagesUpdatesSearcherService(IRepositoryService repositoryService, IAuthenticationSilencerService authenticationSilencerService, IPackageCacheService packageCacheService,
            IRepositoryCacheService repositoryCacheService)
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => authenticationSilencerService);
            Argument.IsNotNull(() => repositoryCacheService);

            _repositoryService = repositoryService;
            _authenticationSilencerService = authenticationSilencerService;
            _packageCacheService = packageCacheService;
            _repositoryCacheService = repositoryCacheService;
        }
        #endregion

        #region Methods
        public IEnumerable<IPackageDetails> SearchForUpdates(bool? allowPrerelease = null, bool authenticateIfRequired = true)
        {
            var availableUpdates = new List<IPackageDetails>();

            using (_authenticationSilencerService.AuthenticationRequiredScope(authenticateIfRequired))
            {
                var packageRepository = _repositoryCacheService.GetNuGetRepository(_repositoryService.GetSourceAggregateRepository());

                var packages = _repositoryCacheService.GetNuGetRepository(_repositoryService.LocalRepository).GetPackages();

                foreach (var package in packages)
                {
                    var prerelease = allowPrerelease ?? package.IsPrerelease();

                    var packageUpdates = packageRepository.GetUpdates(new[] {package}, prerelease, false).Select(x => _packageCacheService.GetPackageDetails(x));
                    availableUpdates.AddRange(packageUpdates);
                }
            }

            return availableUpdates;
        }
        #endregion
    }
}