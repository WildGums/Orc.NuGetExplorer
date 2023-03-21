namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Configuration;

internal static class PackageSourceExtensions
{
    internal static IEnumerable<PackageSource> ToPackageSourceInstances(this IEnumerable<IPackageSource> packageSources)
    {
        ArgumentNullException.ThrowIfNull(packageSources);

        return packageSources.Select(x => new PackageSource(x.Source, x.Name, x.IsEnabled, x.IsOfficial));
    }

    internal static IEnumerable<IPackageSource> ToPackageSourceInterfaces(this IEnumerable<PackageSource> packageSources)
    {
        ArgumentNullException.ThrowIfNull(packageSources);

        return packageSources.Select(x => new NuGetFeed(x.Name, x.Source, x.IsEnabled, x.IsOfficial));
    }
}