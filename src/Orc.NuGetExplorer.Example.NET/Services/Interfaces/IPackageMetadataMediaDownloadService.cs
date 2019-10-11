namespace Orc.NuGetExplorer.Services
{
    using NuGet.Protocol.Core.Types;
    using System.Threading.Tasks;

    public interface IPackageMetadataMediaDownloadService
    {
        Task DownloadFromAsync(IPackageSearchMetadata packageMetadata);
    }
}
