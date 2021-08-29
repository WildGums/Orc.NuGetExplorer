namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Management;

    [ObsoleteEx(TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
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
            Argument.IsNotNull(() => repositoryContextService);
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => defaultExtensibleProjectProvider);
            Argument.IsNotNull(() => repositoryProvider);

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
            // TODO: get repositories based on packageOperationType
            // currenly returns all available repositories
            using (var context = _repositoryContextService.AcquireContext())
            {
                var projects = _extensibleProjectLocator.GetAllExtensibleProjects();

                var localRepos = _projectManager.AsLocalRepositories(projects);

                var repos = context == SourceContext.EmptyContext ? new List<SourceRepository>()
                    : context.ReadAllSourceRepositories();

                var repositoryModelList = new List<IRepository>();

                //wrap all source repository object in repository model
                repositoryModelList.AddRange(
                    repos.Select(
                        source => CreateModelRepositoryFromSourceRepository(source)
                ));

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

        private IRepository CreateModelRepositoryFromSourceRepository(SourceRepository repository)
        {
            return new Repository(repository.PackageSource);
        }
    }
}
