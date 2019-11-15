using System;
using NuGet.Protocol.Core.Types;
using Orc.NuGetExplorer.Enums;
using Orc.NuGetExplorer.Models;

namespace Orc.NuGetExplorer.Pagination
{
    public class DeferToken
    {
        public Func<IPackageSearchMetadata> PackageSelector { get; set; }

        public MetadataOrigin LoadType { get; set; }

        public NuGetPackage Package { get; set; }

        public Action<PackageStatus> UpdateAction { get; set; }
    }
}
