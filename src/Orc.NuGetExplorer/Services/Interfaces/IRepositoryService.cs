namespace Orc.NuGetExplorer;

using System.Collections.Generic;

public interface IRepositoryService
{
    IRepository LocalRepository { get; }

    IReadOnlyList<IRepository> GetRepositories(PackageOperationType packageOperationType);
    IReadOnlyList<IRepository> GetSourceRepositories();
    IReadOnlyList<IRepository> GetUpdateRepositories();
}
