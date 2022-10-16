namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Pagination;
    using Orc.NuGetExplorer.Providers;

    public interface IPackageLoaderService
    {
        IPackageMetadataProvider? PackageMetadataProvider { get; }

        Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token);
    }
}
