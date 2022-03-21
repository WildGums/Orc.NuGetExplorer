namespace Orc.NuGetExplorer.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

#pragma warning disable IDISP025 // Class with no virtual dispose method should be sealed.
    public class SourceContext : IDisposable
#pragma warning restore IDISP025 // Class with no virtual dispose method should be sealed.
    {
        private static readonly Stack<SourceContext> ActiveContext = new Stack<SourceContext>();

        private SourceContext()
        {
            //empty context
        }

        public SourceContext(IReadOnlyList<PackageSource> packageSources)
        {
            PackageSources = packageSources;
            ActiveContext.Push(this);
        }

        public SourceContext(IReadOnlyList<SourceRepository> sourceRepositories)
        {
            Repositories = sourceRepositories;
            ActiveContext.Push(this);
        }

        public static SourceContext EmptyContext { get; set; } = new SourceContext();

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


        public IReadOnlyList<PackageSource> PackageSources { get; private set; }
        public IReadOnlyList<SourceRepository> Repositories { get; private set; }

        public void Dispose()
        {
            if (this != EmptyContext)
            {
                ActiveContext.Pop();
            }
        }
    }
}
