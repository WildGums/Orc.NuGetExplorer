namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using NuGetExplorer.Management;
    using NuGetExplorer.Pagination;
    using NuGetExplorer.Providers;

    internal class LocalPackagesLoaderService : IPackagesLoaderService
    {
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;

        private readonly INuGetPackageManager _projectManager;
        private readonly ISourceRepositoryProvider _repositoryProvider;
        private readonly IRepositoryContextService _repositoryService;

        public Lazy<IPackageMetadataProvider> PackageMetadataProvider { get; set; }

        public LocalPackagesLoaderService(IRepositoryContextService repositoryService, IExtensibleProjectLocator extensibleProjectLocator,
            INuGetPackageManager nuGetExtensibleProjectManager, ISourceRepositoryProvider repositoryProvider)
        {
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => nuGetExtensibleProjectManager);
            Argument.IsNotNull(() => repositoryService);

            _extensibleProjectLocator = extensibleProjectLocator;
            _projectManager = nuGetExtensibleProjectManager;
            _repositoryProvider = repositoryProvider;
            _repositoryService = repositoryService;

            PackageMetadataProvider = new Lazy<IPackageMetadataProvider>(() => InitializeMetdataProvider());
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token)
        {
            Argument.IsValid(nameof(pageContinuation), pageContinuation, pageContinuation.IsValid);

            var source = pageContinuation.Source.PackageSources.FirstOrDefault();
            var observedProjects = _extensibleProjectLocator.GetAllExtensibleProjects();

            SourceRepository repository = null;

            if (source != null)
            {
                repository = _repositoryProvider.CreateRepository(source);
            }
            else
            {
                repository = observedProjects.FirstOrDefault().AsSourceRepository(_repositoryProvider);
            }

            try
            {
                var localPackages = await _projectManager.CreatePackagesCollectionFromProjectsAsync(observedProjects, token);

                var pagedPackages = localPackages
                    .GetLatest(VersionComparer.Default)
                    .Where(package => package.Id.IndexOf(searchTerm ?? String.Empty, StringComparison.OrdinalIgnoreCase) != -1)
                    .OrderBy(package => package.Id)
                    .Skip(pageContinuation.GetNext());


                if (pageContinuation.Size > 0)
                {
                    pagedPackages = pagedPackages.Take(pageContinuation.Size).ToList();
                }

                List<IPackageSearchMetadata> combinedFindedMetadata = new List<IPackageSearchMetadata>();

                foreach (var package in pagedPackages)
                {
                    var metadata = await GetPackageMetadataAsync(package, searchFilter.IncludePrerelease, token);

                    if (metadata != null)
                    {
                        combinedFindedMetadata.Add(metadata);
                    }
                }

                return combinedFindedMetadata;
            }
            catch (FatalProtocolException ex) when (token.IsCancellationRequested)
            {
                //task is cancelled, supress
                throw new OperationCanceledException("Search request was canceled", ex, token);
            }
        }

        public IPackageMetadataProvider InitializeMetdataProvider()
        {
            //todo provide more automatic way
            //create package metadata provider from context
            var context = _repositoryService.AcquireContext();

            var projects = _extensibleProjectLocator.GetAllExtensibleProjects();

            var localRepos = _projectManager.AsLocalRepositories(projects);

            var repos = context.Repositories ?? context.PackageSources?.Select(src => _repositoryService.GetRepository(src)) ?? new List<SourceRepository>();

            return new PackageMetadataProvider(repos, localRepos);
        }

        public async Task<IPackageSearchMetadata> GetPackageMetadataAsync(PackageIdentity identity, bool includePrerelease, CancellationToken cancellationToken)
        {
            // first we try and load the metadata from a local package
            var packageMetadata = await PackageMetadataProvider.Value.GetLocalPackageMetadataAsync(identity, includePrerelease, cancellationToken);

            if (packageMetadata == null)
            {
                // and failing that we go to the network
                packageMetadata = await PackageMetadataProvider.Value.GetPackageMetadataAsync(identity, includePrerelease, cancellationToken);
            }
            return packageMetadata;
        }
    }
}
