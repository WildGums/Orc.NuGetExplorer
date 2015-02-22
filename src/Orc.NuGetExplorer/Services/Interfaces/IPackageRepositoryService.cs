namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using NuGet;

    public interface IPackageRepositoryService
    {
        IPackageRepository GetRepository(string actionName, IEnumerable<PackageSource> packageSources);
    }
}