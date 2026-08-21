namespace Orc.NuGetExplorer.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel;
using Catel.Collections;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGetExplorer.Pagination;
using NuGetExplorer.Providers;

internal class PackagesLoaderService : IPackageLoaderService
{
    private readonly ILogger _nugetLogger;
    private readonly ISourceRepositoryProvider _repositoryProvider;

    public PackagesLoaderService(ISourceRepositoryProvider repositoryProvider, ILogger logger)
    {
        _nugetLogger = logger;
        _repositoryProvider = repositoryProvider;
    }

    public IPackageMetadataProvider? PackageMetadataProvider { get; }

    public async Task<IReadOnlyList<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token)
    {
        if (pageContinuation.Source.PackageSources.Count < 2)
        {
            var packageSource = pageContinuation.Source.PackageSources.FirstOrDefault();
            if (packageSource is null)
            {
                return Array.Empty<IPackageSearchMetadata>();
            }

            var repository = _repositoryProvider.CreateRepository(packageSource);

            try
            {
                var searchResource = await repository.GetResourceAsync<PackageSearchResource>();
                if (searchResource is null)
                {
                    return Array.Empty<IPackageSearchMetadata>();
                }

                var packages = await searchResource.SearchAsync(searchTerm, searchFilter, pageContinuation.GetNext(), pageContinuation.Size, _nugetLogger, token);

                await LoadVersionsEagerIfNeedAsync(searchResource, packages);

                return packages.ToArray();
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

            return packages.ToArray();
        }
    }

    public async Task<IReadOnlyList<IPackageSearchMetadata>> LoadAsyncFromSourcesAsync(string searchTerm, PageContinuation pageContinuation,
        SearchFilter searchFilter, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(pageContinuation);

        SourceRepository? tempRepoLocal = null;

        var repositoryCollection = pageContinuation.Source.PackageSources.Select(source =>
        {
            tempRepoLocal = _repositoryProvider.CreateRepository(source);

            return tempRepoLocal;
        }).ToArray();

        try
        {
            var searchResource = await MultiplySourceSearchResource.CreateAsync(repositoryCollection);

            var packages = await searchResource.SearchAsync(searchTerm, searchFilter, pageContinuation.GetNext(), pageContinuation.Size, _nugetLogger, token);

            return packages.ToArray();
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
