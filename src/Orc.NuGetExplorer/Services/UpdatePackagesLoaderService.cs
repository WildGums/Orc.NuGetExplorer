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
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Packaging;
    using NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Pagination;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    internal class UpdatePackagesLoaderService : IPackageLoaderService, IPackagesUpdatesSearcherService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IRepositoryService _repositoryService;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly INuGetPackageManager _nuGetExtensibleProjectManager;

#pragma warning disable IDISP006 // Implement IDisposable.
        private readonly IServiceLocator _serviceLocator;
#pragma warning restore IDISP006 // Implement IDisposable.

        //underlying service
        private readonly Lazy<IPackageLoaderService> _feedRepositoryLoader;
        private readonly Lazy<IPackageLoaderService> _projectRepositoryLoader;

        private readonly HashSet<string> _discardedPackagesSet = new();

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

            _feedRepositoryLoader = new Lazy<IPackageLoaderService>(() => _serviceLocator.ResolveRequiredType<IPackageLoaderService>());
            _projectRepositoryLoader = new Lazy<IPackageLoaderService>(() => _serviceLocator.ResolveRequiredType<IPackageLoaderService>("Installed"));
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

                var updateList = new List<IPackageSearchMetadata>();

                //getting last metadata
                foreach (var package in installedPackagesMetadatas)
                {
                    if (_discardedPackagesSet.Contains(package.Identity.Id))
                    {
                        continue;
                    }

                    var clonedMetadata = await PackageMetadataProvider.GetHighestPackageMetadataAsync(package.Identity.Id, searchFilter.IncludePrerelease, token);

                    if (clonedMetadata is null)
                    {
                        Log.Warning($"Couldn't retrieve update metadata for installed {package.Identity}");
                        continue;
                    }

                    if (clonedMetadata.Identity.Version > package.Identity.Version)
                    {
                        var combinedMetadata = UpdatePackageSearchMetadataBuilder.FromMetadatas((ClonedPackageSearchMetadata)clonedMetadata, package).Build();
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
            var updateList = new List<IPackageSearchMetadata>();

            try
            {
                Log.Debug("Searching for updates, allowPrerelease = {0}, authenticateIfRequired = {1}", allowPrerelease, authenticateIfRequired);

                // Get local packages
                var localRepo = _repositoryService.LocalRepository;

                var installedPackagesMetadatas = await _projectRepositoryLoader.Value.LoadWithDefaultsAsync(localRepo.Source, token);

                // Getting updates
                foreach (var package in installedPackagesMetadatas)
                {
                    // Current behavior defined based on installed version
                    // Pre-release versions upgraded to latest stable or pre-release
                    // Stable versions upgraded to latest stable only
                    var isPrereleaseUpdate = allowPrerelease ?? package.Identity.Version.IsPrerelease;
                    var clonedMetadata = await PackageMetadataProvider.GetHighestPackageMetadataAsync(package.Identity.Id, isPrereleaseUpdate, token);

                    if (clonedMetadata is null)
                    {
                        Log.Warning($"Couldn't retrieve update metadata for installed {package.Identity}");
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
        }

        public async Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default)
        {

            return (await SearchForPackagesUpdatesAsync(allowPrerelease, authenticateIfRequired, token))
            .Select(
                m => PackageDetailsFactory.Create(PackageOperationType.Update, m, m.Identity, true))
            .ToList();
        }

        public async Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(string[] excludeReleaseTags, bool? allowPrerelease = null, CancellationToken token = default)
        {
            Argument.IsNotNull(() => excludeReleaseTags);

            var foundUpdates = (await SearchForPackagesUpdatesAsync(allowPrerelease, true, token)).ToList();

            // Replace all packages with restricted tag with nearest possible
            var packagesToExclude = foundUpdates.Where(p => p.Identity.Version.Release.ContainsAny(excludeReleaseTags, StringComparison.OrdinalIgnoreCase)).ToList();
            var metadataProvider = PackageMetadataProvider;

            if (packagesToExclude.Any())
            {
                var localRepositorySource = _repositoryService.LocalRepository.Source;
                var installedPackagesMetadatas = await _projectRepositoryLoader.Value.LoadWithDefaultsAsync(localRepositorySource, token);

                foreach (var package in packagesToExclude)
                {
                    foundUpdates.Remove(package);
                    var metadata = await metadataProvider.GetHighestPackageMetadataAsync(package.Identity.Id, allowPrerelease ?? true, excludeReleaseTags, token);
                    if (metadata is null)
                    {
                        Log.Debug($"Couldn't retrieve update metadata for installed package {package.Identity.Id}");
                        continue;
                    }

                    var localPackage = installedPackagesMetadatas.FirstOrDefault(p => string.Equals(p.Identity.Id, metadata.Identity.Id));
                    if (localPackage is null)
                    {
                        // Normally shouldn't happen
                        Log.Debug($"Couldn't match retrieved update metadata with any local package");
                        continue;
                    }
                    if (metadata.Identity.Version > localPackage.Identity.Version)
                    {
                        // Add as replacement for alpha
                        foundUpdates.Add(metadata);
                    }
                }
            }

            var updatePackages = foundUpdates.Select(p => PackageDetailsFactory.Create(PackageOperationType.Update, p, p.Identity, true)).ToList();
            return updatePackages;
        }
        #endregion
    }
}
