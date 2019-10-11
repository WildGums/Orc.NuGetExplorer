using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Packaging;
using Orc.NuGetExplorer.Pagination;
using Orc.NuGetExplorer.Providers;

namespace Orc.NuGetExplorer.Services
{
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Management;
    using NuGetExplorer.Packaging;
    using NuGetExplorer.Pagination;
    using NuGetExplorer.Providers;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    public class UpdatePackagesLoaderService : IPackagesLoaderService
    {
        private static ILog Log = LogManager.GetCurrentClassLogger();

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
    }
}
