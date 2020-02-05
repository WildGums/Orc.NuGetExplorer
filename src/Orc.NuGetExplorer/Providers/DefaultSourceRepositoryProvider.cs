namespace Orc.NuGetExplorer.Providers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using NuGet.Configuration;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Models;

    public class DefaultSourceRepositoryProvider : IExtendedSourceRepositoryProvider
    {
        private static readonly IEnumerable<Lazy<INuGetResourceProvider>> V3ProtocolProviders = Repository.Provider.GetCoreV3();

        private readonly INuGetSettings _settings;
        private readonly INuGetConfigurationService _nuGetConfigurationService;

        private readonly ConcurrentDictionary<PackageSource, SourceRepository> _repositoryStore = new ConcurrentDictionary<PackageSource, SourceRepository>(DefaultNuGetComparers.PackageSource);

        /// <summary>
        /// Unused provider from NuGet library
        /// </summary>
        public IPackageSourceProvider PackageSourceProvider => null;

        public DefaultSourceRepositoryProvider(IModelProvider<ExplorerSettingsContainer> settingsProvider, INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => settingsProvider);
            _settings = settingsProvider.Model;
            _nuGetConfigurationService = nuGetConfigurationService;
        }

        public SourceRepository CreateRepository(PackageSource source)
        {
            var repo = _repositoryStore.GetOrAdd(source, (sourcekey) => new SourceRepository(source, V3ProtocolProviders, FeedType.Undefined));
            return repo;
        }

        public SourceRepository CreateRepository(PackageSource source, FeedType type)
        {
            var repo = _repositoryStore.GetOrAdd(source, (sourcekey) => new SourceRepository(source, V3ProtocolProviders, type));
            return repo;
        }

        public IEnumerable<SourceRepository> GetRepositories()
        {
            List<SourceRepository> repos = new List<SourceRepository>();

            //from config
            var configuredSources = _nuGetConfigurationService.LoadPackageSources(true)
                .ToPackageSourceInstances().ToList();

            //constructed manually from outside 
            //todo add this to model settings right after creating
            //foreach(var storedSource in _repositoryStore.Keys)
            //{
            //    if(configuredSources.FirstOrDefault(source => source.Name == storedSource.Name) == null)
            //    {
            //        configuredSources.Add(storedSource);
            //    }
            //}

            //from settings model
            foreach (var source in _settings.GetAllPackageSources())
            {
                repos.Add(CreateRepository(source));
            }

            foreach (var configSource in configuredSources)
            {
                if (repos.FirstOrDefault(source => source.PackageSource.Name == configSource.Name) == null)
                {
                    repos.Add(CreateRepository(configSource));
                }
            }

            //this provider aware of same-source repositories
            repos = repos.Distinct(DefaultNuGetComparers.SourceRepository).ToList();

            return repos;
        }

        public SourceRepository CreateLocalRepository(string source)
        {
            return new SourceRepository(
                        new PackageSource(source), Repository.Provider.GetCoreV3(), FeedType.FileSystemV2
                );
        }
    }
}
