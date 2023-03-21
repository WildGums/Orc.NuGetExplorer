namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Protocol.Core.Types;
using Orc.NuGetExplorer.Management;

internal class RepositoryService : IRepositoryService
{
    private readonly IRepositoryContextService _repositoryContextService;
    private readonly IExtensibleProjectLocator _extensibleProjectLocator;
    private readonly INuGetPackageManager _projectManager;
    private readonly INuGetConfigurationService _nuGetConfigurationService;
    private readonly IDefaultExtensibleProjectProvider _defaultExtensibleProjectProvider;
    private readonly ISourceRepositoryProvider _repositoryProvider;

    public RepositoryService(IRepositoryContextService repositoryContextService, IExtensibleProjectLocator extensibleProjectLocator,
        INuGetPackageManager projectManager, INuGetConfigurationService nuGetConfigurationService,
        IDefaultExtensibleProjectProvider defaultExtensibleProjectProvider, ISourceRepositoryProvider repositoryProvider)
    {
        ArgumentNullException.ThrowIfNull(repositoryContextService);
        ArgumentNullException.ThrowIfNull(extensibleProjectLocator);
        ArgumentNullException.ThrowIfNull(projectManager);
        ArgumentNullException.ThrowIfNull(nuGetConfigurationService);
        ArgumentNullException.ThrowIfNull(defaultExtensibleProjectProvider);
        ArgumentNullException.ThrowIfNull(repositoryProvider);

        _repositoryContextService = repositoryContextService;
        _extensibleProjectLocator = extensibleProjectLocator;
        _projectManager = projectManager;
        _nuGetConfigurationService = nuGetConfigurationService;
        _defaultExtensibleProjectProvider = defaultExtensibleProjectProvider;
        _repositoryProvider = repositoryProvider;

        LocalRepository = GetMainProjectRepository();
    }

    public IRepository LocalRepository { get; }

    public IEnumerable<IRepository> GetRepositories(PackageOperationType packageOperationType)
    {
        // Todo get repositories based on packageOperationType
        // currenly returns all available repositories
        // create package metadata provider from context
        using (var context = _repositoryContextService.AcquireContext())
        {
            var projects = _extensibleProjectLocator.GetAllExtensibleProjects();

            var localRepos = _projectManager.AsLocalRepositories(projects);

            var repos = context == SourceContext.EmptyContext ? new List<SourceRepository>()
                : context.Repositories ?? context.PackageSources?.Select(src => _repositoryContextService.GetRepository(src));

            var repositoryModelList = new List<IRepository>();

            // wrap all source repository object in repository model
            if (repos is not null)
            {
                repositoryModelList.AddRange(repos.Select(
                    source => CreateModelRepositoryFromSourceRepository(source)
                ));
            }

            repositoryModelList.AddRange(
                localRepos.Select(
                    source => CreateModelRepositoryFromSourceRepository(source)

                ));

            return repositoryModelList;
        }
    }

    public IEnumerable<IRepository> GetSourceRepositories()
    {
        return GetRepositories(PackageOperationType.None);
    }

    public IEnumerable<IRepository> GetUpdateRepositories()
    {
        return GetRepositories(PackageOperationType.Update);
    }

    private IRepository GetMainProjectRepository()
    {
        var repository = _defaultExtensibleProjectProvider.GetDefaultProject().AsSourceRepository(_repositoryProvider);

        return CreateModelRepositoryFromSourceRepository(repository);
    }

    private IRepository CreateModelRepositoryFromSourceRepository(SourceRepository? repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        return new Repository(repository.PackageSource.Source)
        {
            Id = 0,
            Name = repository.PackageSource.Name,
            OperationType = PackageOperationType.None
        };
    }
}