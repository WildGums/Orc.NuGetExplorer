namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Catel;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;

    public static class PackageSearchMetadataExtensions
    {
        public static IEnumerable<VersionInfo> ToVersionInfo(this IEnumerable<IPackageSearchMetadata> packages, bool includePrerelease)
        {
            return packages?
                .Where(v => includePrerelease || !v.Identity.Version.IsPrerelease)
                .OrderByDescending(m => m.Identity.Version, VersionComparer.VersionRelease)
                .Select(m => new VersionInfo(m.Identity.Version, m.DownloadCount)
                {
                    PackageSearchMetadata = m
                });
        }

        public static IPackageSearchMetadata Highest(this IEnumerable<IPackageSearchMetadata> packages, bool includePrerelease, CancellationToken cancellationToken)
        {
            Argument.IsNotNull(() => packages);

            var master = packages.OrderByDescending(x => x.Identity.Version).FirstOrDefault();
            return master?.WithVersions(() => packages.ToVersionInfo(includePrerelease));
        }

        public static IPackageSearchMetadata Highest(this IEnumerable<IPackageSearchMetadata> packages, bool includePrerelease, string[] ignoreReleases, CancellationToken cancellationToken)
        {
            Argument.IsNotNull(() => packages);
            Argument.IsNotNull(() => ignoreReleases);

            var master = packages.OrderByDescending(x => x.Identity.Version).FirstOrDefault(x => !x.Identity.Version.Release.ContainsAny(ignoreReleases, StringComparison.OrdinalIgnoreCase));
            return master?.WithVersions(() => packages.ToVersionInfo(includePrerelease));
        }
    }
}
