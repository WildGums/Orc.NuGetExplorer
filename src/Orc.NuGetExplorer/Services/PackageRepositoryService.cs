// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageRepositoryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.ExceptionHandling;
    using Catel.Logging;
    using NuGet;
    using Repositories;

    internal class PackageRepositoryService : IPackageRepositoryService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly IPackageRepositoryFactory _repositoryFactory;
        #endregion

        #region Constructors
        public PackageRepositoryService(INuGetConfigurationService nuGetConfigurationService, IPackageRepositoryFactory packageRepositoryFactory)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => packageRepositoryFactory);

            _nuGetConfigurationService = nuGetConfigurationService;
            _repositoryFactory = packageRepositoryFactory;
        }
        #endregion

        private IEnumerable<IPackageSource> GetPackageSources()
        {
            return _nuGetConfigurationService.LoadPackageSources();
        }

        #region Methods
        public IDictionary<string, IPackageRepository> GetRepositories(RepositoryCategoryType category)
        {
            var packageSources = GetPackageSources();
            var result = new Dictionary<string, IPackageRepository>();
            switch (category)
            {
                case RepositoryCategoryType.Installed:
                    result[RepoName.All] = GetLocalRepository();
                    break;

                case RepositoryCategoryType.Online:
                    result[RepoName.All] = new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true);
                    var remoteRepositories = GetRemoteRepositories();
                    result.AddRange(remoteRepositories);
                    break;

                case RepositoryCategoryType.Update:
                    result[RepoName.All] = GetAggeregateUpdateRepository();
                    var updateRepositories = GetUpdateRepositories();
                    result.AddRange(updateRepositories);
                    break;
            }

            return result;
        }

        public IDictionary<string, IPackageRepository> GetUpdateRepositories()
        {
            var localRepository = GetLocalRepository();
            return GetRemoteRepositories().ToDictionary(x => x.Key, x => (IPackageRepository) new UpdateRepository(localRepository, x.Value));
        }

        public IPackageRepository GetAggregateRepository()
        {
            var packageSources = GetPackageSources();
            return new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true);
        }

        public IDictionary<string, IPackageRepository> GetRemoteRepositories()
        {
            var result = new Dictionary<string, IPackageRepository>();
            var packageSources = GetPackageSources();
            foreach (var packageSource in packageSources)
            {               
                var repo = _repositoryFactory.CreateRepository(packageSource.Source);

                result.Add(packageSource.Name, repo);
            }

            return result;
        }

        public IPackageRepository GetLocalRepository()
        {
            var path = _nuGetConfigurationService.GetDestinationFolder();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return new LocalPackageRepository(path, true);
        }

        private IPackageRepository GetAggeregateUpdateRepository()
        {
            var localRepository = GetLocalRepository();
            var packageSources = GetPackageSources();
            var sourceRepository = new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true);
            return new UpdateRepository(localRepository, sourceRepository);
        }
        #endregion
    }
}