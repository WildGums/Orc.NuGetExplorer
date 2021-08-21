namespace Orc.NuGetExplorer.Services
{
    using Catel;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Management;

    internal class RepositoryContextService : IRepositoryContextService
    {
        private readonly ISourceRepositoryProvider _sourceRepositoryProvider;

        public RepositoryContextService(ISourceRepositoryProvider sourceRepositoryProvider)
        {
            Argument.IsNotNull(() => sourceRepositoryProvider);

            _sourceRepositoryProvider = sourceRepositoryProvider;
        }

        public SourceRepository GetRepository(PackageSource source)
        {
            return _sourceRepositoryProvider.TryGetRepository(source);
        }

        public SourceContext AcquireContext(PackageSource source)
        {
            return SourceContext.AcquireContext(source);
        }

        public SourceContext AcquireContext(bool ignoreLocal = false)
        {
            return SourceContext.AcquireContext(false);
        }
    }
}
