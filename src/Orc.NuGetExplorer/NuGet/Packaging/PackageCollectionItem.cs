namespace Orc.NuGetExplorer.Packaging;

using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

public sealed class PackageCollectionItem : PackageIdentity
{
    /// <summary>
    /// Installed package references.
    /// </summary>
    public List<PackageReference> PackageReferences { get; }

    public PackageCollectionItem(string id, NuGetVersion version, IEnumerable<PackageReference> installedReferences)
        : base(id, version)
    {
        PackageReferences = installedReferences?.ToList() ?? new List<PackageReference>();
    }
}