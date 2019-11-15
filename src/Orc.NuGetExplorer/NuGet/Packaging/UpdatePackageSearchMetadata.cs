namespace Orc.NuGetExplorer.Packaging
{
    using NuGet.Protocol.Core.Types;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    public class UpdatePackageSearchMetadata : ClonedPackageSearchMetadata
    {
        public VersionInfo FromVersion { get; set; }
    }
}
