namespace Orc.NuGetExplorer
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Management;

    [ObsoleteEx(ReplacementTypeOrMember = "SourceContext", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
    public interface IRepositoryContextService
    {
        [ObsoleteEx(ReplacementTypeOrMember = "ISourceRepositoryProviderExtensions.TryGetRepository", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        SourceRepository GetRepository(PackageSource source);
        [ObsoleteEx(ReplacementTypeOrMember = "SourceContext.AcquireContext", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        SourceContext AcquireContext(PackageSource source);
        [ObsoleteEx(ReplacementTypeOrMember = "SourceContext.AcquireContext", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        SourceContext AcquireContext(bool ignoreLocal = false);
    }
}
