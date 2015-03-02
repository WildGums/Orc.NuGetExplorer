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
    using NuGet;

    public class PackageRepositoryService : IPackageRepositoryService
    {
        #region Fields
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly PackageSource[] _packageSources;
        private readonly PackageRepositoryFactory _repositoryFactory;
        #endregion

        #region Constructors
        public PackageRepositoryService(IPackageSourceService packageSourceService, INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => packageSourceService);
            Argument.IsNotNull(() => nuGetConfigurationService);

            _nuGetConfigurationService = nuGetConfigurationService;
            _repositoryFactory = PackageRepositoryFactory.Default;
            _packageSources = packageSourceService.GetPackageSources().ToArray();
        }
        #endregion

        #region Methods
        public IDictionary<string, IPackageRepository> GetRepositories(RepoCategoryType category)
        {
            var result = new Dictionary<string, IPackageRepository>();
            switch (category)
            {
                case RepoCategoryType.Installed:
                    var folder = _nuGetConfigurationService.GetDestinationFolder();
                    result[RepoName.All] = GetLocalRepository(folder);
                    break;
                case RepoCategoryType.Online:
                    result[RepoName.All] = new AggregateRepository(_repositoryFactory, _packageSources.Select(x => x.Source), true);
                    var remoteRepositories = GetRemoteRepositories();
                    result.AddRange(remoteRepositories);
                    return remoteRepositories;
            }

            return result;
        }

        public IPackageRepository GetAggregateRepository()
        {
            return new AggregateRepository(_repositoryFactory, _packageSources.Select(x => x.Source), true);
        }

        public IDictionary<string, IPackageRepository> GetRemoteRepositories()
        {
            var result = new Dictionary<string, IPackageRepository>();
            foreach (var packageSource in _packageSources)
            {
                var repo = _repositoryFactory.CreateRepository(packageSource.Source);
                result.Add(packageSource.Name, repo);
            }

            return result;
        }

        public IPackageRepository GetLocalRepository(string path)
        {
            Argument.IsNotNullOrEmpty(() => path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return new LocalPackageRepository(path, true);
        }
        #endregion
    }
}