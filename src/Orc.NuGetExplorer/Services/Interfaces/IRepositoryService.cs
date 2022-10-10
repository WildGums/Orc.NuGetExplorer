namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IRepositoryService
    {
        IRepository LocalRepository { get; }

        IEnumerable<IRepository> GetRepositories(PackageOperationType packageOperationType);
        IEnumerable<IRepository> GetSourceRepositories();
        IRepository GetSourceAggregateRepository();
        IEnumerable<IRepository> GetUpdateRepositories();
        IRepository GetUpdateAggeregateRepository();
    }
}
