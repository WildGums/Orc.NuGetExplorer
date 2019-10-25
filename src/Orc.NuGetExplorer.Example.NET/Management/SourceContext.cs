using Orc.NuGetExplorer.Services;

namespace Orc.NuGetExplorer.Management
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SourceContext : IDisposable
    {
        private static Stack<SourceContext> _activeContext = new Stack<SourceContext>();

        public static SourceContext CurrentContext
        {
            get
            {
                if (!_activeContext.Any())
                {
                    return null;
                }

                return _activeContext.Peek();
            }
        }

        public SourceContext(IReadOnlyList<PackageSource> packageSources, IRepositoryService repositoryService)
        {
            PackageSources = packageSources;
            _activeContext.Push(this);
        }

        public SourceContext(IReadOnlyList<SourceRepository> sourceRepositories, IRepositoryService repositoryService)
        {
            Repositories = sourceRepositories;
            _activeContext.Push(this);
        }


        public IReadOnlyList<PackageSource> PackageSources { get; private set; }
        public IReadOnlyList<SourceRepository> Repositories { get; private set; }

        public void Dispose()
        {
            //todo release this context
            _activeContext.Pop();
        }
    }
}
