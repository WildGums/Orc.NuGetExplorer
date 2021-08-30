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

        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly ISourceRepositoryProvider _sourceRepositoryProvider;

        // underlying service
        private readonly Lazy<IPackageLoaderService> _projectRepositoryPackageLoader;

        private readonly HashSet<string> _discardedPackagesSet = new();

        public UpdatePackagesLoaderService(IExtensibleProjectLocator extensibleProjectLocator, ISourceRepositoryProvider sourceRepositoryProvider, IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => sourceRepositoryProvider);
            Argument.IsNotNull(() => serviceLocator);

            _extensibleProjectLocator = extensibleProjectLocator;
            _sourceRepositoryProvider = sourceRepositoryProvider;

            _projectRepositoryPackageLoader = new Lazy<IPackageLoaderService>(() => serviceLocator.ResolveType<IPackageLoaderService>("Installed"));
        }

        [ObsoleteEx(ReplacementTypeOrMember = "SourceContext.PackageMetadataProvider", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public IPackageMetadataProvider PackageMetadataProvider => _projectRepositoryPackageLoader.Value.PackageMetadataProvider;

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

                var installedPackagesMetadatas = await _projectRepositoryPackageLoader.Value.LoadAsync(searchTerm, localContinuation, searchFilter, token);

                pageContinuation.GetNext();

                Log.Info("Local packages queryed for further available update searching");

                List<IPackageSearchMetadata> updateList = new List<IPackageSearchMetadata>();

                using (var sourceContext = SourceContext.AcquireContext())
                {
                    var packageMetadataProvider = sourceContext.PackageMetadataProviderValue;
                    //getting last metadata
                    foreach (var package in installedPackagesMetadatas)
                    {
          
                        if (_discardedPackagesSet.Contains(package.Identity.Id))
                        {
                            continue;
                        }

                        var clonedMetadata = (await packageMetadataProvider.GetPackageMetadataListAsync(package.Identity.Id, searchFilter.IncludePrerelease, false, token))
                            .Highest(searchFilter.IncludePrerelease, token);

                        if (clonedMetadata is null)
                        {
                            Log.Warning($"Couldn't retrieve update metadata for installed {package.Identity}");
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
            var emptySearchTerm = string.Empty;

            try
            {
                using (var sourceContext = SourceContext.AcquireContext())
                {
                    Log.Debug("Searching for updates, allowPrerelease = {0}, authenticateIfRequired = {1}", allowPrerelease, authenticateIfRequired);

                    var packageMetadataProvider = sourceContext.PackageMetadataProviderValue;

                    // Get local packages
                    var localFilter = new SearchFilter(true);

                    // Note: This code need to be changes in the future if multiple local installation folder (projects) will supported
                    var localRepository = _extensibleProjectLocator.GetDefaultProject().AsSourceRepository(_sourceRepositoryProvider);
                    var localPagination = new PageContinuation(0, new PackageSourceWrapper(localRepository.PackageSource.Source));

                    var installedPackagesMetadatas = await _projectRepositoryPackageLoader.Value.LoadAsync(emptySearchTerm, localPagination, localFilter, token);

                    // Getting updatess
                    foreach (var package in installedPackagesMetadatas)
                    {
                        // Current behavior defined based on installed version
                        // pre-release versions upgraded to latest stable or pre-release
                        // stable versions upgraded to latest stable only
                        var isPrereleaseUpdate = allowPrerelease ?? package.Identity.Version.IsPrerelease;

                        var clonedMetadata = (await packageMetadataProvider.GetPackageMetadataListAsync(package.Identity.Id, isPrereleaseUpdate, false, token))
                            .Highest(isPrereleaseUpdate, token);
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
            }
            catch (Exception ex) when (token.IsCancellationRequested)
            {
                throw new OperationCanceledException("Search request was canceled", ex, token);
            }
        }

        public async Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default)
        {

            return (await SearchForPackagesUpdatesAsync(allowPrerelease, authenticateIfRequired, token)).Select(
                m => PackageDetailsFactory.Create(PackageOperationType.Update, m, m.Identity, true))
            .ToList();
        }
        #endregion
    }
}
