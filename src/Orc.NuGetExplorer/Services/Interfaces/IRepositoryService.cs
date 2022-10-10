namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IRepositoryService
    {
        #region Properties
        IRepository LocalRepository { get; }
        #endregion

        #region Methods
        IEnumerable<IRepository> GetRepositories(PackageOperationType packageOperationType);
        IEnumerable<IRepository> GetSourceRepositories();
        IRepository GetSourceAggregateRepository();
        IEnumerable<IRepository> GetUpdateRepositories();
        IRepository GetUpdateAggeregateRepository();
        #endregion
    }
}