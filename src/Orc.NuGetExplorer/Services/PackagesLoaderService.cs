namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Pagination;
    using NuGetExplorer.Providers;

    internal class PackagesLoaderService : IPackagesLoaderService
    {
        public Lazy<IPackageMetadataProvider> PackageMetadataProvider { get; set; }

        public async Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token)
        {
            Argument.IsValid(nameof(pageContinuation), pageContinuation, pageContinuation.IsValid);

            if (pageContinuation.Source.PackageSources.Count < 2)
            {
                var repository = new SourceRepository(pageContinuation.Source.PackageSources.FirstOrDefault(), Repository.Provider.GetCoreV3());


                var searchResource = await repository.GetResourceAsync<PackageSearchResource>();
                var httpHandler = await repository.GetResourceAsync<HttpHandlerResourceV3>();

                try
                {
                    var packages = await searchResource.SearchAsync(searchTerm, searchFilter, pageContinuation.GetNext(), pageContinuation.Size, new Loggers.DebugLogger(true), token);

                    return packages;
                }
                catch (FatalProtocolException ex) when (token.IsCancellationRequested)
                {
                    //task is cancelled, supress
                    throw new OperationCanceledException("Search request was canceled", ex, token);
                }
                finally
                {
                    var credentialsService = httpHandler.GetCredentialServiceImplementation<ExplorerCredentialService>();

                    if (credentialsService != null)
                    {
                        credentialsService.ClearRetryCache();
                    }
                }
            }
            else
            {
                var packages = await LoadAsyncFromSources(searchTerm, pageContinuation, searchFilter, token);

                return packages;
            }
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> LoadAsyncFromSources(string searchTerm, PageContinuation pageContinuation,
            SearchFilter searchFilter, CancellationToken token)
        {
            SourceRepository tempRepoLocal = null;

            var searchResource = await MultiplySourceSearchResource.CreateAsync(
                pageContinuation.Source.PackageSources.Select(s =>
                    {
                        tempRepoLocal = new SourceRepository(s, Repository.Provider.GetCoreV3());
                        return tempRepoLocal;
                    })
                .ToArray());

            var httpHandler = await tempRepoLocal?.GetResourceAsync<HttpHandlerResourceV3>();

            try
            {
                var packages = await searchResource.SearchAsync(searchTerm, searchFilter,
                    pageContinuation.GetNext(), pageContinuation.Size, new Loggers.DebugLogger(true), token);

                return packages;
            }
            catch (FatalProtocolException ex) when (token.IsCancellationRequested)
            {
                //task is cancelled, supress
                throw new OperationCanceledException("Search request was cancelled", ex, token);
            }
            finally
            {
                var credentialsService = httpHandler.GetCredentialServiceImplementation<ExplorerCredentialService>();

                if (credentialsService != null)
                {
                    credentialsService.ClearRetryCache();
                }
            }
        }
    }
}
