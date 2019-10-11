using Orc.NuGetExplorer.Management;

namespace Orc.NuGetExplorer.Services
{
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Management;

    public interface IRepositoryService
    {
        SourceRepository GetRepository(PackageSource source);
        SourceContext AcquireContext(PackageSource source);
        SourceContext AcquireContext();

    }
}
