namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    internal class UpdatePackagesLoaderService : IPackageLoaderService, IPackagesUpdatesSearcherService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IRepositoryService _repositoryService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly INuGetPackageManager _nuGetExtensibleProjectManager;

        private readonly IServiceLocator _serviceLocator;

        //underlying service
        private readonly Lazy<IPackageLoaderService> _feedRepositoryLoader;
        private readonly Lazy<IPackageLoaderService> _projectRepositoryLoader;

        private readonly HashSet<string> _discardedPackagesSet = new HashSet<string>();

        public UpdatePackagesLoaderService(IRepositoryService repositoryService, IExtensibleProjectLocator extensibleProjectLocator,
           INuGetPackageManager nuGetExtensibleProjectManager)
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => nuGetExtensibleProjectManager);

            _repositoryService = repositoryService;
            _extensibleProjectLocator = extensibleProjectLocator;
            _nuGetExtensibleProjectManager = nuGetExtensibleProjectManager;

            _serviceLocator = this.GetServiceLocator();

            _feedRepositoryLoader = new Lazy<IPackageLoaderService>(() => _serviceLocator.ResolveType<IPackageLoaderService>());
            _projectRepositoryLoader = new Lazy<IPackageLoaderService>(() => _serviceLocator.ResolveType<IPackageLoaderService>("Installed"));
        }

        public IPackageMetadataProvider PackageMetadataProvider => _projectRepositoryLoader.Value.PackageMetadataProvider;

        public async Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token)
        {
            try
            {
                if (pageContinuation.Current <= 0)
                {
                    //start search from begin, don't skip packages
                    _discardedPackagesSet.Clear();
                }

                var localContinuation = new PageContinuation(pageContinuation, !pageContinuation.Source.PackageSources.Any());

                var installedPackagesMetadatas = (await _projectRepositoryLoader.Value.LoadAsync(searchTerm, localContinuation, searchFilter, token));

                pageContinuation.GetNext();

                Log.Info("Local packages queryed for further available update searching");

                List<IPackageSearchMetadata> updateList = new List<IPackageSearchMetadata>();

                //getting last metadata
                foreach (var package in installedPackagesMetadatas)
                {
                    if (_discardedPackagesSet.Contains(package.Identity.Id))
                    {
                        continue;
                    }

                    var clonedMetadata = await PackageMetadataProvider.GetHighestPackageMetadataAsync(package.Identity.Id, searchFilter.IncludePrerelease, token);

                    if (clonedMetadata == null)
                    {
                        Log.Error($"Couldn't retrieve update metadata for installed {package.Identity}");
                        continue;
                    }

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

        public async Task<IEnumerable<IPackageSearchMetadata>> SearchForPackagesUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default)
        {
            //todo auth scopes?
            var scopeManagers = new List<ScopeManager<AuthenticationScope>>();

            var updateList = new List<IPackageSearchMetadata>();
            var emptySearchTerm = String.Empty;

            try
            {
                Log.Debug("Searching for updates, allowPrerelease = {0}, authenticateIfRequired = {1}", allowPrerelease, authenticateIfRequired);

                //get local packages
                var localFilter = new SearchFilter(true);
                var localRepo = _repositoryService.LocalRepository;
                var localPagination = new PageContinuation(0, new PackageSourceWrapper(localRepo.Source));

                var installedPackagesMetadatas = await _projectRepositoryLoader.Value.LoadAsync(emptySearchTerm, localPagination, localFilter, token);

                //getting updates
                foreach (var package in installedPackagesMetadatas)
                {
                    //current behavior defined based on installed version
                    //pre-release versions upgraded to latest stable or pre-release
                    //stable versions upgraded to latest stable only
                    var isPrereleaseUpdate = allowPrerelease ?? package.Identity.Version.IsPrerelease;
                    var clonedMetadata = await PackageMetadataProvider.GetHighestPackageMetadataAsync(package.Identity.Id, isPrereleaseUpdate, token);

                    if (clonedMetadata == null)
                    {
                        Log.Error($"Couldn't retrieve update metadata for installed {package.Identity}");
                        continue;
                    }

                    if (clonedMetadata.Identity.Version > package.Identity.Version)
                    {
                        //construct package details
                        updateList.Add(clonedMetadata);
                    }
                }

                return updateList;
            }
            catch (Exception ex) when (token.IsCancellationRequested)
            {
                throw new OperationCanceledException("Search request was canceled", ex, token);
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

        public async Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default)
        {

            return (await SearchForPackagesUpdatesAsync(allowPrerelease, authenticateIfRequired, token))
            .Select(
                m => PackageDetailsFactory.Create(PackageOperationType.Update, m, m.Identity, true))
            .ToList();
        }
        #endregion
    }
}
