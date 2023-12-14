namespace Orc.NuGetExplorer;

using System;
using System.Linq;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Packaging;

public static class PackageCollectionExtensions
{
    public static PackageIdentity[] GetLatest(this PackageCollection packages, IVersionComparer versionComparer)
    {
        ArgumentNullException.ThrowIfNull(packages);
        ArgumentNullException.ThrowIfNull(versionComparer);

        return packages
            .GroupBy(p => p.Id, p => p.Version, StringComparer.OrdinalIgnoreCase)
            //max or default
            .Select(g => new PackageIdentity(
                g.Key,
                g.OrderByDescending(v => v, versionComparer)
                    .FirstOrDefault()))
            .ToArray();
    }
}
