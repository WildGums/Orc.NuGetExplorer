namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Scoping;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Packaging;
    using NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Pagination;
    using Orc.NuGetExplorer.Scopes;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    internal class UpdatePackagesLoaderService : IPackagesLoaderService, IPackagesUpdatesSearcherService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IRepositoryService _repositoryService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly INuGetExtensibleProjectManager _nuGetExtensibleProjectManager;

        private readonly IServiceLocator _serviceLocator;

        //underlying service
        private readonly Lazy<IPackagesLoaderService> _feedRepositoryLoader;
        private readonly Lazy<IPackagesLoaderService> _projectRepositoryLoader;

        private readonly HashSet<string> _discardedPackagesSet = new HashSet<string>();

        public UpdatePackagesLoaderService(IRepositoryService repositoryService, IExtensibleProjectLocator extensibleProjectLocator,
           INuGetExtensibleProjectManager nuGetExtensibleProjectManager)
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => nuGetExtensibleProjectManager);

            _repositoryService = repositoryService;
            _extensibleProjectLocator = extensibleProjectLocator;
            _nuGetExtensibleProjectManager = nuGetExtensibleProjectManager;

            _serviceLocator = this.GetServiceLocator();

            _feedRepositoryLoader = new Lazy<IPackagesLoaderService>(() => _serviceLocator.ResolveType<IPackagesLoaderService>());
            _projectRepositoryLoader = new Lazy<IPackagesLoaderService>(() => _serviceLocator.ResolveType<IPackagesLoaderService>("Installed"));
        }

        public Lazy<IPackageMetadataProvider> PackageMetadataProvider { get; set; }

        public async Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token)
        {
            try
            {
                if (pageContinuation.Current <= 0)
                {
                    //start search from begin, don't skip packages
                    _discardedPackagesSet.Clear();
                }

                var localContinuation = new PageContinuation(pageContinuation);

                var installedPackagesMetadatas = (await _projectRepositoryLoader.Value.LoadAsync(searchTerm, localContinuation, searchFilter, token));

                pageContinuation.GetNext();

                Log.Info("Local packages queryed for further available update searching");

                if (PackageMetadataProvider == null)
                {
                    PackageMetadataProvider = _projectRepositoryLoader.Value.PackageMetadataProvider;
                }

                List<IPackageSearchMetadata> updateList = new List<IPackageSearchMetadata>();

                //getting last metadata
                foreach (var package in installedPackagesMetadatas)
                {
                    if (_discardedPackagesSet.Contains(package.Identity.Id))
                    {
                        continue;
                    }

                    var clonedMetadata = await PackageMetadataProvider.Value.GetHighestPackageMetadataAsync(package.Identity.Id, searchFilter.IncludePrerelease, token);

                    if (clonedMetadata.Identity.Version > package.Identity.Version)
                    {
                        var combinedMetadata = UpdatePackageSearchMetadataBuilder.FromMetadatas(clonedMetadata as ClonedPackageSearchMetadata, package).Build();
                        updateList.Add(combinedMetadata);
                    }

                    _discardedPackagesSet.Add(package.Identity.Id);


                    if (updateList.Count >= pageContinuation.Size)
                    {
                        break;
                    }
                }

                return updateList;
            }
            catch (Exception ex) when (token.IsCancellationRequested)
            {
                throw new OperationCanceledException("Search request was canceled", ex, token);
            }
        }

        #region IPackagesUpdatesSearcherService
        public IEnumerable<IPackageDetails> SearchForUpdates(bool? allowPrerelease = null, bool authenticateIfRequired = true)
        {
            //todo auth scopes?
            var scopeManagers = new List<ScopeManager<AuthenticationScope>>();

            try
            {
                Log.Debug("Searching for updates, allowPrerelease = {0}, authenticateIfRequired = {1}", allowPrerelease, authenticateIfRequired);

                /*               
                foreach (var repository in sourceRepositories)
                {
                    var scopeManager = ScopeManager<AuthenticationScope>.GetScopeManager(repository.Source.GetSafeScopeName(), () => new AuthenticationScope(authenticateIfRequired));
                    scopeManagers.Add(scopeManager);
                }
                */

                var repositories = _repositoryService.GetSourceRepositories();

                //TODO
                //1. Mark repository in repository services by Operation correctly
                //2. Filter project repositories
                //3. Find update by package metadata provider and filter
                //4. Create PackageDetails from metadatas

                /*
                var packageRepository = _repositoryCacheService.GetNuGetRepository(_repositoryService.GetSourceAggregateRepository());
                var packages = _repositoryCacheService.GetNuGetRepository(_repositoryService.LocalRepository).GetPackages();

                foreach (var package in packages)
                {
                    var prerelease = allowPrerelease ?? package.IsPrerelease();

                    var packageUpdates = packageRepository.GetUpdates(new[] { package }, prerelease, false).Select(x => _packageCacheService.GetPackageDetails(packageRepository, x, allowPrerelease ?? true));
                    availableUpdates.AddRange(packageUpdates);
                }
                */

                Log.Debug("Finished searching for updates, found '{0}' updates"); //availableUpdates.Count);

                return null;
            }
            finally
            {
                foreach (var scopeManager in scopeManagers)
                {
                    scopeManager.Dispose();
                }

                scopeManagers.Clear();
            }
        }
        #endregion
    }
}
