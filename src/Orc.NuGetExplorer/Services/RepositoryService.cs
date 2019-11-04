namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Services;

    internal class RepositoryService : IRepositoryService
    {
        private readonly IRepositoryContextService _repositoryContextService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly INuGetExtensibleProjectManager _projectManager;
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly IDefaultExtensibleProjectProvider _defaultExtensibleProjectProvider;
        private readonly ISourceRepositoryProvider _repositoryProvider;

        public RepositoryService(IRepositoryContextService repositoryContextService, IExtensibleProjectLocator extensibleProjectLocator,
            INuGetExtensibleProjectManager projectManager, INuGetConfigurationService nuGetConfigurationService,
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

        public IRepository GetSourceAggregateRepository()
        {
            //var packageSource = new CombinedNuGetSource(GetSourceRepositories()
            //    .Select(x => new NuGetFeed(x.Name, x.Source, x.));

            //var allInOneSource = new CombinedNuGetSource(;

            throw new NotImplementedException();
        }

        public IEnumerable<IRepository> GetSourceRepositories()
        {
            NuGetFeed temp = null;

            var feedList = new List<NuGetFeed>();

            //todo temp cast
            var configurationService = _nuGetConfigurationService as NugetConfigurationService;

            var keyCollection = configurationService.GetAllKeys(ConfigurationContainer.Roaming);

            for (int i = 0; i < keyCollection.Count; i++)
            {
                temp = configurationService.GetRoamingValue(keyCollection[i]);

                if (temp != null)
                {
                    feedList.Add(temp);
                }
            }

            var repositories = feedList.Select(feed => new Repository()
            {
                Id = 0,
                OperationType = PackageOperationType.None,
                Name = feed.Name,
                Source = feed.Source
            });

            return repositories;
        }



        public IRepository GetUpdateAggeregateRepository()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRepository> GetUpdateRepositories()
        {
            throw new NotImplementedException();
        }

        private IRepository GetMainProjectRepository()
        {
            var repository = _defaultExtensibleProjectProvider.GetDefaultProject().AsSourceRepository(_repositoryProvider);

            return CreateModelRepositoryFromSourceRepository(repository);
        }
    }
}
