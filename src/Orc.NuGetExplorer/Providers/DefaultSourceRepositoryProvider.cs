namespace Orc.NuGetExplorer.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using NuGet.Configuration;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Models;

    public class DefaultSourceRepositoryProvider : ISourceRepositoryProvider
    {
        private readonly IEnumerable<Lazy<INuGetResourceProvider>> _resourceProviders;

        private readonly INuGetSettings _settings;
        private readonly INuGetConfigurationService _nuGetConfigurationService;

        /// <summary>
        /// Unused provider from NuGet library
        /// </summary>
        public IPackageSourceProvider PackageSourceProvider => null;

        public DefaultSourceRepositoryProvider(IModelProvider<ExplorerSettingsContainer> settingsProvider, INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => settingsProvider);
            _resourceProviders = Repository.Provider.GetCoreV3();
            _settings = settingsProvider.Model;
            _nuGetConfigurationService = nuGetConfigurationService;
        }

        public SourceRepository CreateRepository(PackageSource source)
        {
            return CreateRepository(source, FeedType.Undefined);
        }

        public SourceRepository CreateRepository(PackageSource source, FeedType type)
        {
            return new SourceRepository(source, _resourceProviders, type);
        }

        public IEnumerable<SourceRepository> GetRepositories()
        {
            List<SourceRepository> repos = new List<SourceRepository>();

            //from config
            var configuredSources = _nuGetConfigurationService.LoadPackageSources(true)
                .Select(feed => new PackageSource(feed.Source, feed.Name, feed.IsEnabled));

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

            return repos;
        }
    }
}
