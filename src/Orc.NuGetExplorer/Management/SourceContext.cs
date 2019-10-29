namespace Orc.NuGetExplorer.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

    public class SourceContext : IDisposable
    {
        private static readonly Stack<SourceContext> ActiveContext = new Stack<SourceContext>();

        public static SourceContext CurrentContext
        {
            get
            {
                if (!ActiveContext.Any())
                {
                    return null;
                }

                return ActiveContext.Peek();
            }
        }

        public SourceContext(IReadOnlyList<PackageSource> packageSources, IRepositoryContextService repositoryService)
        {
            PackageSources = packageSources;
            ActiveContext.Push(this);
        }

        public SourceContext(IReadOnlyList<SourceRepository> sourceRepositories, IRepositoryContextService repositoryService)
        {
            Repositories = sourceRepositories;
            ActiveContext.Push(this);
        }

        public IReadOnlyList<PackageSource> PackageSources { get; private set; }
        public IReadOnlyList<SourceRepository> Repositories { get; private set; }

        public void Dispose()
        {
            //todo release this context
            ActiveContext.Pop();
        }
    }
}
