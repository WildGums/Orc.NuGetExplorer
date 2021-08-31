namespace Orc.NuGetExplorer.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.IoC;
    using Catel.Scoping;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Providers;

    public sealed class SourceContext : IDisposable
    {
        private readonly ScopeManager<Sources> _scopeManager;
        private readonly ISourceRepositoryProvider _sourceRepositoryProvider;
        private readonly ITypeFactory _typeFactory;
        private readonly Lazy<IPackageMetadataProvider> _packageMetadataProviderLazyInitializer;

        private SourceContext()
        {

        }

        private SourceContext(ScopeManager<Sources> scopeManager, ISourceRepositoryProvider sourceRepositoryProvider, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => scopeManager);
            Argument.IsNotNull(() => sourceRepositoryProvider);
            Argument.IsNotNull(() => typeFactory);

            _scopeManager = scopeManager;
            _sourceRepositoryProvider = sourceRepositoryProvider;
            _typeFactory = typeFactory;

            _packageMetadataProviderLazyInitializer = new Lazy<IPackageMetadataProvider>(
                _typeFactory.CreateInstanceWithParametersAndAutoCompletion<PackageMetadataProvider>(this)
                );
        }

        public static SourceContext EmptyContext { get; set; } = new SourceContext();

        public Sources Source
        {
            get
            {
                var scopeManager = _scopeManager;
                if (scopeManager.RefCount == 0 && scopeManager.ScopeObject is null)
                {
                    // context is disposed
                    throw new ObjectDisposedException(nameof(SourceContext));
                }

                return scopeManager.ScopeObject;
            }
        }

        public IPackageMetadataProvider PackageMetadataProviderValue => _packageMetadataProviderLazyInitializer.Value;

        public IEnumerable<SourceRepository> ReadAllSourceRepositories()
        {
            var instantiatedRepositories = new List<SourceRepository>(Source.Repositories);
            var repositoryFromPackageSources = Source.PackageSources.Select(src => _sourceRepositoryProvider.TryGetRepository(src)).
#pragma warning disable CL0006 // Using "is" statement inside null comparison expression is recommended style
                Where(repository => repository != null);
#pragma warning restore CL0006 // Using "is" statement inside null comparison expression is recommended style

            instantiatedRepositories.AddRange(repositoryFromPackageSources);
            return instantiatedRepositories;
        }

        public static SourceContext AcquireContext(bool ignoreLocal = false)
        {
            var scope = ignoreLocal ? FeedScope.LocalExcluded : FeedScope.All;

            var sourceRepositoryProvider = ServiceLocator.Default.ResolveType<ISourceRepositoryProvider>();
            var typeFactory = ServiceLocator.Default.ResolveType<ITypeFactory>();

            IReadOnlyList<SourceRepository> repositories = sourceRepositoryProvider.GetRepositories()
                .Where(r => !r.PackageSource.IsLocal || !ignoreLocal).ToList();

            if (repositories.Any())
            {
                var scopeManager = ScopeManager<Sources>.GetScopeManager(scope.ToString(), () => new Sources(repositories));
                return new SourceContext(scopeManager, sourceRepositoryProvider, typeFactory);
            }

            return EmptyContext;
        }

        public static SourceContext AcquireContext(PackageSource source)
        {
            var scope = FeedScope.Single;

            var sourceRepositoryProvider = ServiceLocator.Default.ResolveType<ISourceRepositoryProvider>();
            var typeFactory = ServiceLocator.Default.ResolveType<ITypeFactory>();

            var repository = sourceRepositoryProvider.TryGetRepository(source);

            if (repository is null)
            {
                return EmptyContext;
            }

            var scopeManager = ScopeManager<Sources>.GetScopeManager(scope.ToString(), () => new Sources(new List<SourceRepository>() { repository }));
            return new SourceContext(scopeManager, sourceRepositoryProvider, typeFactory);
        }

        public void Dispose()
        {
            _scopeManager.Dispose();
        }

        public class Sources
        {
            private readonly List<PackageSource> _packageSources = new();
            private readonly List<SourceRepository> _sourceRepositories = new();

            public Sources()
            {

            }

            public Sources(IReadOnlyList<PackageSource> packageSources)
            {
                _packageSources.AddRange(packageSources);
            }

            public Sources(IReadOnlyList<SourceRepository> sourceRepositories)
            {
                _sourceRepositories.AddRange(sourceRepositories);
            }

            public IReadOnlyList<PackageSource> PackageSources => _packageSources;
            public IReadOnlyList<SourceRepository> Repositories => _sourceRepositories;
        }

        internal enum FeedScope
        {
            All = 1,
            LocalExcluded = 2,
            Single = 3
        }
    }
}
