using Orc.NuGetExplorer.Models;

namespace Orc.NuGetExplorer.Providers
{
    using Catel;
    using NuGet.Configuration;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Models;
    using System;
    using System.Collections.Generic;

    public class SourceRepositoryProvider : ISourceRepositoryProvider
    {
        private readonly IEnumerable<Lazy<INuGetResourceProvider>> _resourceProviders;

        private readonly INuGetSettings _settings;

        /// <summary>
        /// Unused prvider from NuGet liabrary
        /// </summary>
        public IPackageSourceProvider PackageSourceProvider => null;

        public SourceRepositoryProvider(IModelProvider<ExplorerSettingsContainer> settingsProvider)
        {
            Argument.IsNotNull(() => settingsProvider);
            _resourceProviders = Repository.Provider.GetCoreV3();
            _settings = settingsProvider.Model;
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

            foreach (var source in _settings.GetAllPackageSources())
            {
                repos.Add(CreateRepository(source));
            }

            return repos;
        }
    }
}
