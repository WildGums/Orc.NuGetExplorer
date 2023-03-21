namespace Orc.NuGetExplorer;

using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;

public interface IPackageMetadataMediaDownloadService
{
    Task DownloadMediaForMetadataAsync(IPackageSearchMetadata packageMetadata);
}