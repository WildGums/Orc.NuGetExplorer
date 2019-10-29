namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Providers;

    internal class RepositoryService : IRepositoryService
    {
        private readonly IRepositoryContextService _repositoryContextService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly INuGetExtensibleProjectManager _projectManager;

        public IRepository LocalRepository => throw new NotImplementedException();

        public RepositoryService(IRepositoryContextService repositoryContextService, IExtensibleProjectLocator extensibleProjectLocator, 
            INuGetExtensibleProjectManager projectManager)
        {
            Argument.IsNotNull(() => repositoryContextService);
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => projectManager);

            _repositoryContextService = repositoryContextService;
            _extensibleProjectLocator = extensibleProjectLocator;
            _projectManager = projectManager;
        }


        public IEnumerable<IRepository> GetRepositories(PackageOperationType packageOperationType)
        {
            //todo provide more automatic way
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
            throw new NotImplementedException();
        }

        public IEnumerable<IRepository> GetSourceRepositories()
        {
            throw new NotImplementedException();
        }

        public IRepository GetUpdateAggeregateRepository()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRepository> GetUpdateRepositories()
        {
            throw new NotImplementedException();
        }
    }
}
