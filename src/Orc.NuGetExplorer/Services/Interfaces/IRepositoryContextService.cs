namespace Orc.NuGetExplorer
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Management;

    public interface IRepositoryContextService
    {
        SourceRepository GetRepository(PackageSource source);
        SourceContext AcquireContext(PackageSource source);
        SourceContext AcquireContext(bool ignoreLocal = false);

    }
}
