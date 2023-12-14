namespace Orc.NuGetExplorer.Packaging;

using System.Threading.Tasks;
using Catel.Logging;
using NuGet.Protocol.Core.Types;
using NuGetExplorer.Enums;

public class NuGetPackageCombinator
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Combines NuGet Package with other metadata
    /// and produce state from their relation
    /// </summary>
    public static async Task<PackageStatus> CombineAsync(NuGetPackage package, MetadataOrigin tokenPage, IPackageSearchMetadata metadata)
    {
        if (tokenPage == MetadataOrigin.Browse)
        {
            await package.MergeMetadataAsync(metadata, tokenPage);

            //keep installed version same, because this NuGetPackage
            //created from local installed nupkg metadata.
        }

        if (tokenPage == MetadataOrigin.Installed)
        {
            //then original package retrived from real source and should be merged with
            //installed local metadata

            await package.MergeMetadataAsync(metadata, tokenPage);

            if (metadata.Identity.HasVersion)
            {
                package.InstalledVersion = metadata.Identity.Version;
            }
            else
            {
                Log.Warning("Package merged metadata from installed package doesn't have package version");
            }
        }

        if (tokenPage == MetadataOrigin.Updates || package.InstalledVersion is null)
        {
            return PackageStatus.NotInstalled;
        }

        var comparison = package.InstalledVersion.CompareTo(package.LastVersion, NuGet.Versioning.VersionComparison.VersionRelease);

        if (comparison <= (int)PackageStatus.NotInstalled || comparison >= (int)PackageStatus.Pending)
        {
            //because of version comparer fallen back to StringComparison of non-numeric labels.

            Log.Debug($"Two packages was compared by release labels with result: {comparison}");

            return comparison < 0 ? PackageStatus.UpdateAvailable : PackageStatus.LastVersionInstalled;
        }

        return (PackageStatus)comparison;
    }
}