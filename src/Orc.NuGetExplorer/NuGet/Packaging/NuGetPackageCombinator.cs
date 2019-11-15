namespace Orc.NuGetExplorer.Packaging
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Enums;
    using NuGetExplorer.Models;

    public class NuGetPackageCombinator
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Combines NuGet Package with other metadata
        /// and produce state from their relation
        /// </summary>
        public static async Task<PackageStatus> Combine(NuGetPackage package, MetadataOrigin tokenPage, IPackageSearchMetadata metadata)
        {
            Argument.IsNotNull(() => metadata);
            Argument.IsNotNull(() => package);

            if (tokenPage == MetadataOrigin.Browse)
            {
                await package.MergeMetadata(metadata, tokenPage);

                //keep installed version same, because this NuGetPackage
                //created from local installed nupkg metadata.
            }

            if (tokenPage == MetadataOrigin.Installed)
            {
                //then original package retrived from real source and should be merged with
                //installed local metadata

                await package.MergeMetadata(metadata, tokenPage);

                if (metadata.Identity.HasVersion)
                {
                    package.InstalledVersion = metadata.Identity.Version;
                }
                else
                {
                    Log.Warning("Package merged metadata from installed package doesn't have package version");
                }
            }

            if (tokenPage == MetadataOrigin.Updates)
            {
                return PackageStatus.NotInstalled;
            }

            var comparison = package.InstalledVersion.CompareTo(package.LastVersion, NuGet.Versioning.VersionComparison.VersionRelease);

            var status = (PackageStatus)comparison == PackageStatus.Pending ? PackageStatus.LastVersionInstalled : (PackageStatus)comparison;

            return status;
        }
    }
}
