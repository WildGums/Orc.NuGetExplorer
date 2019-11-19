namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
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
            //todo get repositories based on packageOperationType
            //currenly returns all available repositories
            //create package metadata provider from context
            using (var context = _repositoryContextService.AcquireContext())
            {
                var projects = _extensibleProjectLocator.GetAllExtensibleProjects();

                var localRepos = _projectManager.AsLocalRepositories(projects);

                var repos = context.Repositories ?? context.PackageSources.Select(src => _repositoryContextService.GetRepository(src));

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


        public IRepository GetSourceAggregateRepository()
        {
            //todo
            //var packageSource = new CombinedNuGetSource(GetSourceRepositories()
            //    .Select(x => new NuGetFeed(x.Name, x.Source, x.));

            //var allInOneSource = new CombinedNuGetSource(;
            return null;
        }

        public IEnumerable<IRepository> GetSourceRepositories()
        {
            return GetRepositories(PackageOperationType.None);
        }


        public IRepository GetUpdateAggeregateRepository()
        {
            //todo
            return null;
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
            return new Repository()
            {
                Id = 0, //todo create serializable repos
                Name = repository.PackageSource.Name,
                Source = repository.PackageSource.Source,
                OperationType = PackageOperationType.None
            };
        }
    }
}
