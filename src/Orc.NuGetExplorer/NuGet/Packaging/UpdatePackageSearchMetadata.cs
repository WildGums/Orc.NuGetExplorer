namespace Orc.NuGetExplorer.Packaging
{
    using System.Collections.Generic;
    using NuGet.Common;
    using NuGet.Protocol.Core.Types;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    public class UpdatePackageSearchMetadata : ClonedPackageSearchMetadata
    {
        public VersionInfo FromVersion { get; set; }
        public AsyncLazy<IEnumerable<VersionInfo>> LazyVersionsFactory { get; set; } 
    }
}
