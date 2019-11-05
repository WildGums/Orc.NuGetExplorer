namespace Orc.NuGetExplorer.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Management;

    internal class RepositoryContextService : IRepositoryContextService
    {
        private readonly ISourceRepositoryProvider _sourceRepositoryProvider;
        private readonly Dictionary<PackageSource, SourceRepository> _constructedRepositories = new Dictionary<PackageSource, SourceRepository>();


        public RepositoryContextService(ISourceRepositoryProvider sourceRepositoryProvider)
        {
            Argument.IsNotNull(() => sourceRepositoryProvider);

            _sourceRepositoryProvider = sourceRepositoryProvider;
        }

        public SourceRepository GetRepository(PackageSource source)
        {
            if (source is null)
            {
                return null;
            }

            SourceRepository sourceRepo = null;

            if (!_constructedRepositories.TryGetValue(source, out sourceRepo))
            {
                sourceRepo = _sourceRepositoryProvider.CreateRepository(source);
            }

            return sourceRepo;
        }

        public SourceContext AcquireContext(PackageSource source)
        {
            var repo = GetRepository(source);

            if(repo == null)
            {
                return SourceContext.EmptyContext;
            }

            var context = new SourceContext(new List<SourceRepository>() { repo });

            return context;
        }

        public SourceContext AcquireContext()
        {
            //acquire for all by default
            IReadOnlyList<SourceRepository> repos = _sourceRepositoryProvider.GetRepositories().ToList();

            var context = new SourceContext(repos);

            return context;
        }
    }
}
