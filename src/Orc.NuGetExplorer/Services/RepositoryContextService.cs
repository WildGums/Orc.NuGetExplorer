namespace Orc.NuGetExplorer.Services
{
    using Catel;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Management;

    [ObsoleteEx(ReplacementTypeOrMember = "SourceContext", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
    internal class RepositoryContextService : IRepositoryContextService
    {
        private readonly ISourceRepositoryProvider _sourceRepositoryProvider;

        public RepositoryContextService(ISourceRepositoryProvider sourceRepositoryProvider)
        {
            Argument.IsNotNull(() => sourceRepositoryProvider);

            _sourceRepositoryProvider = sourceRepositoryProvider;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "ISourceRepositoryProviderExtensions.TryGetRepository", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public SourceRepository GetRepository(PackageSource source)
        {
            return _sourceRepositoryProvider.TryGetRepository(source);
        }

        [ObsoleteEx(ReplacementTypeOrMember = "SourceContext.AcquireContext", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public SourceContext AcquireContext(PackageSource source)
        {
            return SourceContext.AcquireContext(source);
        }

        [ObsoleteEx(ReplacementTypeOrMember = "SourceContext.AcquireContext", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public SourceContext AcquireContext(bool ignoreLocal = false)
        {
            return SourceContext.AcquireContext(false);
        }
    }
}
