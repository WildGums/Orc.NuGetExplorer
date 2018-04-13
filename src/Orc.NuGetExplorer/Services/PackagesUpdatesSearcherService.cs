// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesUpdatesSearcherService.cs" company="WildGums">
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
    using Catel.Scoping;
    using NuGet;
    using Scopes;

    internal class PackagesUpdatesSearcherService : IPackagesUpdatesSearcherService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IPackageCacheService _packageCacheService;
        private readonly IRepositoryService _repositoryService;
        private readonly IRepositoryCacheService _repositoryCacheService;
        #endregion

        #region Constructors
        public PackagesUpdatesSearcherService(IRepositoryService repositoryService, IPackageCacheService packageCacheService,
            IRepositoryCacheService repositoryCacheService)
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => repositoryCacheService);

            _repositoryService = repositoryService;
            _packageCacheService = packageCacheService;
            _repositoryCacheService = repositoryCacheService;
        }
        #endregion

        #region Methods
        public IEnumerable<IPackageDetails> SearchForUpdates(bool? allowPrerelease = null, bool authenticateIfRequired = true)
        {
            var scopeManagers = new List<ScopeManager<AuthenticationScope>>();

            try
            {
                Log.Debug("Searching for updates, allowPrerelease = {0}, authenticateIfRequired = {1}", allowPrerelease, authenticateIfRequired);

                var sourceRepositories = _repositoryService.GetSourceRepositories();
                foreach (var repository in sourceRepositories)
                {
                    var scopeManager = ScopeManager<AuthenticationScope>.GetScopeManager(repository.Source.GetSafeScopeName(), () => new AuthenticationScope(authenticateIfRequired));
                    scopeManagers.Add(scopeManager);
                }

                var availableUpdates = new List<IPackageDetails>();
                var packageRepository = _repositoryCacheService.GetNuGetRepository(_repositoryService.GetSourceAggregateRepository());
                var packages = _repositoryCacheService.GetNuGetRepository(_repositoryService.LocalRepository).GetPackages();

                foreach (var package in packages)
                {
                    var prerelease = allowPrerelease ?? package.IsPrerelease();

                    var packageUpdates = packageRepository.GetUpdates(new[] { package }, prerelease, false).Select(x => _packageCacheService.GetPackageDetails(packageRepository, x, allowPrerelease ?? true));
                    availableUpdates.AddRange(packageUpdates);
                }

                Log.Debug("Finished searching for updates, found '{0}' updates", availableUpdates.Count);

                return availableUpdates;
            }
            finally
            {
                foreach (var scopeManager in scopeManagers)
                {
                    scopeManager.Dispose();
                }

                scopeManagers.Clear();
            }
        }
        #endregion
    }
}