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

    internal class LocalPackagesLoaderService : IPackageLoaderService
    {
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;

        private readonly INuGetPackageManager _projectManager;
        private readonly ISourceRepositoryProvider _repositoryProvider;

        [ObsoleteEx(ReplacementTypeOrMember = "SourceContext.PackageMetadataProvider", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public IPackageMetadataProvider PackageMetadataProvider => throw new NotSupportedException();

        public LocalPackagesLoaderService(IExtensibleProjectLocator extensibleProjectLocator, INuGetPackageManager nuGetExtensibleProjectManager, ISourceRepositoryProvider repositoryProvider)
        {
            Argument.IsNotNull(() => extensibleProjectLocator);
            Argument.IsNotNull(() => nuGetExtensibleProjectManager);

            _extensibleProjectLocator = extensibleProjectLocator;
            _projectManager = nuGetExtensibleProjectManager;
            _repositoryProvider = repositoryProvider;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token)
        {
            Argument.IsValid(nameof(pageContinuation), pageContinuation, pageContinuation.IsValid);

            var source = pageContinuation.Source.PackageSources.FirstOrDefault();
            var observedProjects = _extensibleProjectLocator.GetAllExtensibleProjects();

            try
            {
                var localPackages = await _projectManager.CreatePackagesCollectionFromProjectsAsync(observedProjects, token);

                var pagedPackages = localPackages
                    .GetLatest(VersionComparer.Default)
                    .Where(package => package.Id.IndexOf(searchTerm ?? string.Empty, StringComparison.OrdinalIgnoreCase) != -1)
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

                    if (metadata is not null)
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

        public async Task<IPackageSearchMetadata> GetPackageMetadataAsync(PackageIdentity identity, bool includePrerelease, CancellationToken cancellationToken)
        {
            // first we try and load the metadata from a local package
            using (var context = SourceContext.AcquireContext())
            {
                var packageMetadataProvider = context.PackageMetadataProviderValue;

                var packageMetadata = await packageMetadataProvider.GetLocalPackageMetadataAsync(identity, includePrerelease, cancellationToken);
                if (packageMetadata is null)
                {
                    // fallback network package if local installation exists but package cannot be read
                    packageMetadata = await packageMetadataProvider.GetPackageMetadataAsync(identity, includePrerelease, cancellationToken);
                }
                return packageMetadata;
            }
        }
    }
}
