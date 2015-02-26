namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using NuGet;

    public interface IPackageRepositoryService
    {
        IDictionary<string, IPackageRepository> GetRepositories(string actionName);
    }
}