namespace Orc.NuGetExplorer.Packaging
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;
    using NuGet.Protocol.Core.Types;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    public class UpdatePackageSearchMetadata : ClonedPackageSearchMetadata
    {
        public VersionInfo FromVersion { get; set; }

        public void ResetValidationContext()
        {
            throw new NotImplementedException();
        }
    }
}
