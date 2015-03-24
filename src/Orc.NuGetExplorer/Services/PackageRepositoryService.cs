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
    using Catel.Logging;
    using NuGet;

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

            LocalRepository = GetLocalRepository();
        }
        #endregion

        #region Properties
        public IRepository LocalRepository { get; private set; }
        #endregion

        #region Methods
        public IDictionary<string, IRepository> GetRepositories(PackageOperationType packageOperationType)
        {
            var packageSources = GetPackageSources();
            var result = new Dictionary<string, IRepository>();
            switch (packageOperationType)
            {
                case PackageOperationType.Uninstall:
                    result[RepoName.All] = LocalRepository;
                    break;

                case PackageOperationType.Install:
                    result[RepoName.All] = new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true).ToPublicRepository();
                    var remoteRepositories = GetSourceRepositories();
                    result.AddRange(remoteRepositories);
                    break;

                case PackageOperationType.Update:
                    result[RepoName.All] = GetUpdateAggeregateRepository();
                    var updateRepositories = GetUpdateRepositories();
                    result.AddRange(updateRepositories);
                    break;
            }

            return result;
        }

        public IDictionary<string, IRepository> GetUpdateRepositories()
        {
            var localRepository = GetLocalRepository();
            return GetSourceRepositories().ToDictionary(x => x.Key, x => new UpdateRepository(localRepository.ToNuGetRepository(), x.Value.ToNuGetRepository()).ToPublicRepository());
        }

        public IRepository GetSourceAggregateRepository()
        {
            var packageSources = GetPackageSources();
            return new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true).ToPublicRepository();
        }

        public IDictionary<string, IRepository> GetSourceRepositories()
        {
            var result = new Dictionary<string, IRepository>();
            var packageSources = GetPackageSources();
            foreach (var packageSource in packageSources)
            {
                var repo = _repositoryFactory.CreateRepository(packageSource.Source);

                result.Add(packageSource.Name, repo.ToPublicRepository());
            }

            return result;
        }

        public IRepository GetUpdateAggeregateRepository()
        {
            var localRepository = GetLocalRepository();
            var packageSources = GetPackageSources();
            var sourceRepository = new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true);
            return new UpdateRepository(localRepository.ToNuGetRepository(), sourceRepository).ToPublicRepository();
        }

        private IEnumerable<IPackageSource> GetPackageSources()
        {
            return _nuGetConfigurationService.LoadPackageSources();
        }

        private IRepository GetLocalRepository()
        {
            var path = _nuGetConfigurationService.GetDestinationFolder();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return new LocalPackageRepository(path, true).ToPublicRepository();
        }
        #endregion
    }
}