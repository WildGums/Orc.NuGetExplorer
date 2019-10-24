namespace Orc.NuGetExplorer
{
    extern alias v_3;

    using NuGet.Protocol.Core.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using v_3::NuGet.Common;

    public class MultiplySourceSearchResource : PackageSearchResource
    {
        private PackageSearchResource[] _combinedResources;

        private MultiplySourceSearchResource()
        {
        }

        private async Task LoadResources(SourceRepository[] sourceRepositories)
        {
            var combinedResourcesTasks = sourceRepositories.Select(x => x.GetResourceAsync<PackageSearchResource>()).ToList();
            var completed = (await Task.WhenAll(combinedResourcesTasks)).Where(m => m != null);
            _combinedResources = completed.ToArray();
        }

        public async static Task<MultiplySourceSearchResource> CreateAsync(SourceRepository[] sourceRepositories)
        {
            var searchRes = new MultiplySourceSearchResource();
            await searchRes.LoadResources(sourceRepositories);

            return searchRes;
        }

        public override async Task<IEnumerable<IPackageSearchMetadata>> SearchAsync(string searchTerm, SearchFilter filters, int skip, int take, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                var searchTasks = _combinedResources.Select(res => res.SearchAsync(searchTerm, filters, skip, take, log, cancellationToken));

                var results = await Task.WhenAll(searchTasks);

                var combinedResults = results.SelectMany(x => x.Select(i => i));

                return await MergeVersionsAsync(combinedResults);
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
    }
}
