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
        [ObsoleteEx(ReplacementTypeOrMember = "SourceContext.PackageMetadataProvider", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        IPackageMetadataProvider PackageMetadataProvider { get; }

        Task<IEnumerable<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token);
    }
}
