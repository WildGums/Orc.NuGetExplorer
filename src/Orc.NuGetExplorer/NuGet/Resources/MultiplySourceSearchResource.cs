namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;

    public class MultiplySourceSearchResource : PackageSearchResource
    {
        private Dictionary<SourceRepository, PackageSearchResource> _resolvedResources;
        private bool _v2Used;

        private MultiplySourceSearchResource()
        {
        }

        public async static Task<MultiplySourceSearchResource> CreateAsync(SourceRepository[] sourceRepositories)
        {
            var searchRes = new MultiplySourceSearchResource();
            await searchRes.LoadResources(sourceRepositories);

            return searchRes;
        }

        private async Task LoadResources(SourceRepository[] sourceRepositories)
        {
            await ResolveResources(sourceRepositories);

            _v2Used = _resolvedResources.Values.Any(resource => resource is PackageSearchResourceV2Feed);
        }

        /// <summary>
        /// Get optimized resources
        /// </summary>
        private async Task ResolveResources(SourceRepository[] sourceRepositories)
        {
            //get one source for repositories with same uri
            var combinedResourcesTasks = sourceRepositories.Distinct(new UniqueSourceSourceRepositoryComparer())
                .Select(async x =>
                {
                    var resource = await x.GetResourceAsync<PackageSearchResource>();
                    return new KeyValuePair<SourceRepository, PackageSearchResource>(x, resource);
                }).ToList();

            var taskResults = (await combinedResourcesTasks.WhenAllOrException()).Where(x => x.IsSuccess)
               .Select(x => x.UnwrapResult());

            _resolvedResources = taskResults.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
        }


        public override async Task<IEnumerable<IPackageSearchMetadata>> SearchAsync(string searchTerm, SearchFilter filters, int skip, int take, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                var resources = _resolvedResources.Values;

                var searchTasks = resources.Select(res => res.SearchAsync(searchTerm, filters, skip, take, log, cancellationToken));

                var results = await Task.WhenAll(searchTasks);

                var combinedResults = results.SelectMany(metadataCollection => metadataCollection.Select(metadata => metadata));

                var mergedResults =  await MergeVersionsAsync(combinedResults);

                if(_v2Used)
                {
                    //load all versions early
                    foreach (var package in mergedResults)
                    {
                        await package.GetVersionsAsync();
                    }
                }

                return mergedResults;
            }
            catch (FatalProtocolException ex) when (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException("Search request was cancelled", ex, cancellationToken);
            }
        }

        private async Task<IEnumerable<IPackageSearchMetadata>> MergeVersionsAsync(IEnumerable<IPackageSearchMetadata> metadatas)
        {
            var identityGroups = metadatas.GroupBy(x => x.Identity.Id)
                .OrderByDescending(x => x.Max(e => e.DownloadCount))
                .Select(g => g.OrderByDescending(e => e.Identity.Version))
                .SelectMany(g => g).Distinct(new PackageIdentityEqualityComparer());


            return identityGroups.ToList();
        }

        private class PackageIdentityEqualityComparer : IEqualityComparer<IPackageSearchMetadata>
        {
            public bool Equals(IPackageSearchMetadata x, IPackageSearchMetadata y)
            {
                return x.Identity.Equals(y.Identity);
            }

            public int GetHashCode(IPackageSearchMetadata obj)
            {
                return obj.Identity.GetHashCode();
            }
        }

        private class UniqueSourceSourceRepositoryComparer : IEqualityComparer<SourceRepository>
        {
            public bool Equals(SourceRepository x, SourceRepository y)
            {
                return DefaultNuGetComparers.PackageSource.Equals(x.PackageSource, y.PackageSource);
            }

            public int GetHashCode(SourceRepository obj)
            {
                return DefaultNuGetComparers.PackageSource.GetHashCode(obj.PackageSource);
            }
        }
    }
}
