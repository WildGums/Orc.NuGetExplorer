namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Pagination;
    using NuGetExplorer.Providers;

    internal class PackagesLoaderService : IPackageLoaderService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ILogger _nugetLogger;
        private readonly ISourceRepositoryProvider _repositoryProvider;

        public PackagesLoaderService(ISourceRepositoryProvider repositoryProvider, ILogger logger)
        {
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => repositoryProvider);

            _nugetLogger = logger;
            _repositoryProvider = repositoryProvider;
        }

        public IPackageMetadataProvider PackageMetadataProvider { get; }

        public async Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token)
        {
            Argument.IsValid(nameof(pageContinuation), pageContinuation, pageContinuation.IsValid);

            if (pageContinuation.Source.PackageSources.Count < 2)
            {
                var repository = _repositoryProvider.CreateRepository(pageContinuation.Source.PackageSources.FirstOrDefault());

                try
                {
                    var searchResource = await repository.GetResourceAsync<PackageSearchResource>();

                    var packages = await searchResource.SearchAsync(searchTerm, searchFilter, pageContinuation.GetNext(), pageContinuation.Size, _nugetLogger, token);

                    await LoadVersionsEagerIfNeedAsync(searchResource, packages);

                    return packages;
                }
                catch (FatalProtocolException ex) when (token.IsCancellationRequested)
                {
                    //task is cancelled, supress
                    throw new OperationCanceledException("Search request was canceled", ex, token);
                }
            }
            else
            {
                var packages = await LoadAsyncFromSourcesAsync(searchTerm, pageContinuation, searchFilter, token);

                return packages;
            }
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> LoadAsyncFromSourcesAsync(string searchTerm, PageContinuation pageContinuation,
            SearchFilter searchFilter, CancellationToken token)
        {
            SourceRepository tempRepoLocal = null;

            var repositoryCollection = pageContinuation.Source.PackageSources.Select(source =>
            {
                tempRepoLocal = _repositoryProvider.CreateRepository(source);

                return tempRepoLocal;
            }).ToArray();

            try
            {
                var searchResource = await MultiplySourceSearchResource.CreateAsync(repositoryCollection);

                var packages = await searchResource.SearchAsync(searchTerm, searchFilter, pageContinuation.GetNext(), pageContinuation.Size, _nugetLogger, token);

                return packages;
            }
            catch (FatalProtocolException ex) when (token.IsCancellationRequested)
            {
                //task is cancelled, supress
                throw new OperationCanceledException("Search request was cancelled", ex, token);
            }
        }

        private async Task LoadVersionsEagerIfNeedAsync(PackageSearchResource searchResource, IEnumerable<IPackageSearchMetadata> packages)
        {

            if (searchResource is PackageSearchResourceV2Feed)
            {
                foreach (var package in packages)
                {
                    await V2SearchHelper.GetVersionsMetadataAsync(package);
                }
            }
        }
    }
}
