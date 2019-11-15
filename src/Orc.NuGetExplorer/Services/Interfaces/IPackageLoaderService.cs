namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Pagination;

    public interface IPackageLoaderService
    {
        System.Lazy<Providers.IPackageMetadataProvider> PackageMetadataProvider { get; set; }

        Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token);
    }
}
