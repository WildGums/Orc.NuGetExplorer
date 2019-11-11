// --------------------------------------------------------------------------------------------------------------------
// <copyright file="repositoryService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.IoC;
    using NuGet;

    internal class RepositoryService : IRepositoryService
    {
        #region Fields
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly IRepositoryCacheService _repositoryCacheService;
        private readonly IPackageRepositoryFactory _repositoryFactory;
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        public RepositoryService(INuGetConfigurationService nuGetConfigurationService, IPackageRepositoryFactory packageRepositoryFactory,
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
                return _repositoryCacheService.GetSerializableRepository(sourceRepository.Name, sourceRepository.Source, PackageOperationType.Update, 
                    () => _typeFactory.CreateInstanceWithParametersAndAutoCompletion<UpdateRepository>(LocalRepository, sourceRepository));
            });
        }

        public IRepository GetSourceAggregateRepository()
        {            
            return _repositoryCacheService.GetSerializableRepository(RepositoryName.All, "all", PackageOperationType.Install,
                () =>
                {
                    var packageSources = GetPackageSources();
                    return new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true);
                }, true);
        }

        public IEnumerable<IRepository> GetSourceRepositories()
        {
            var packageSources = GetPackageSources();
            foreach (var packageSource in packageSources)
            {
                var source = packageSource;
                yield return _repositoryCacheService.GetSerializableRepository(packageSource.Name, packageSource.Source, PackageOperationType.Install, () => _repositoryFactory.CreateRepository(source.Source));
            }
        }

        public IRepository GetUpdateAggeregateRepository()
        {            
            return _repositoryCacheService.GetSerializableRepository(RepositoryName.All, "all", PackageOperationType.Update, () =>
            {
                var packageSources = GetPackageSources();
                var sourceRepository = new AggregateRepository(_repositoryFactory, packageSources.Select(x => x.Source), true);
                return new UpdateRepository(LocalNuGetRepository, sourceRepository);
            }, true);
        }

        private IEnumerable<IPackageSource> GetPackageSources()
        {
            return _nuGetConfigurationService.LoadPackageSources(true);
        }

        private IRepository GetLocalRepository()
        {
            return _repositoryCacheService.GetSerializableRepository(RepositoryName.All, "all", PackageOperationType.Uninstall, () =>
            {
                var path = _nuGetConfigurationService.GetDestinationFolder();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return new LocalPackageRepository(path, true);
            });
        }
        #endregion
    }
}
