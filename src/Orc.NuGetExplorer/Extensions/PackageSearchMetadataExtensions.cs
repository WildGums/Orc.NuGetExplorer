namespace Orc.NuGetExplorer
{
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using System.Collections.Generic;
    using System.Linq;

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
    }
}
