namespace Orc.NuGetExplorer.Pagination
{
    using System;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Enums;
    using Orc.NuGetExplorer.Models;

    public class DeferToken
    {
        public Func<IPackageSearchMetadata> PackageSelector { get; set; }

        /// <summary>
        /// Determines type of source which should be sought for state acquiring
        /// </summary>
        public MetadataOrigin LoadType { get; set; }

        public NuGetPackage Package { get; set; }

        public IPackageSearchMetadata Result { get; set; }

        public Action<PackageStatus> UpdateAction { get; set; }
    }
}
