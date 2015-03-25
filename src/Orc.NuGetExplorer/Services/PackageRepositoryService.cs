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
    using Catel.IoC;
    using Catel.Logging;
    using NuGet;

    internal class PackageRepositoryService : IPackageRepositoryService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly IRepositoryCacheService _repositoryCacheService;
        private readonly IPackageRepositoryFactory _repositoryFactory;
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        public PackageRepositoryService(INuGetConfigurationService nuGetConfigurationService, IPackageRepositoryFactory packageRepositoryFactory,
            IRepositoryCacheService repositoryCacheService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => repositoryCacheService);
            Argument.IsNotNull(() => typeFactory);

            _nuGetConfigurationService = nuGetConfigurationService;
            _repositoryFactory = packageRepositoryFactory;
            _repositoryCacheService = repositoryCacheService;
            _typeFactory = typeFactory;

            LocalRepository = GetLocalRepository();
            LocalNuGetRepository = _repositoryCacheService.GetNuGetRepository(LocalRepository);
        }
        #endregion

        #region Properties
        public IRepository LocalRepository { get; private set; }
        private IPackageRepository LocalNuGetRepository { get; set; }
        #endregion

        #region Methods
        public IEnumerable<IRepository> GetRepositories(PackageOperationType packageOperationType)
        {
            var packageSources = GetPackageSources();
            var result = new List<IRepository>();
            switch (packageOperationType)
            {
                case PackageOperationType.Uninstall:
                    result.Add(LocalRepository);
                    break;

                case PackageOperationType.Install:
                    result.Add(GetSourceAggregateRepository());
                    var remoteRepositories = GetSourceRepositories();
                    result.AddRange(remoteRepositories);
                    break;

                case PackageOperationType.Update:
                    result.Add(GetUpdateAggeregateRepository());
                    var updateRepositories = GetUpdateRepositories();
                    result.AddRange(updateRepositories);
                    break;
            }

            return result;
        }

        public IEnumerable<IRepository> GetUpdateRepositories()
        {
            return GetSourceRepositories().Select(sourceRepository =>
            {
                var updateRepository = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<UpdateRepository>(LocalRepository, sourceRepository);
                return _repositoryCacheService.GetSerialisableRepository(sourceRepository.Name, PackageOperationType.Update, updateRepository);
            });
        }

        public IRepository GetSourceAggregateRepository()
        {
            var packageSources = GetPackageSources();
            var repository = new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true);
            return _repositoryCacheService.GetSerialisableRepository(RepositoryName.All, PackageOperationType.Install, repository);
        }

        public IEnumerable<IRepository> GetSourceRepositories()
        {
            var packageSources = GetPackageSources();
            foreach (var packageSource in packageSources)
            {
                var repository = _repositoryFactory.CreateRepository(packageSource.Source);

                yield return _repositoryCacheService.GetSerialisableRepository(packageSource.Name, PackageOperationType.Install, repository);
            }
        }

        public IRepository GetUpdateAggeregateRepository()
        {
            var packageSources = GetPackageSources();
            var sourceRepository = new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true);
            var updateRepository = new UpdateRepository(LocalNuGetRepository, sourceRepository);
            return _repositoryCacheService.GetSerialisableRepository(RepositoryName.All, PackageOperationType.Update, updateRepository);
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

            return _repositoryCacheService.GetSerialisableRepository(RepositoryName.All, PackageOperationType.Uninstall, new LocalPackageRepository(path, true));
        }
        #endregion
    }
}