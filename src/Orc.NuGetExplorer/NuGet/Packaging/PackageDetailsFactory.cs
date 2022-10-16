namespace Orc.NuGetExplorer.Packaging
{
    using System;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;

    /// <summary>
    /// Package metadata builder used for creating metadata implementing old IPackageDetails interface
    /// </summary>
    public class PackageDetailsFactory
    {
        public static IPackageDetails Create(PackageOperationType operationType, IPackageSearchMetadata? versionMetadata, PackageIdentity packageIdentity, bool? isLastVersion)
        {
            ArgumentNullException.ThrowIfNull(versionMetadata);
            
            var packageDetails = new PackageDetails(versionMetadata, packageIdentity, isLastVersion ?? false);
            if (operationType != PackageOperationType.Install)
            {
                packageDetails.IsInstalled = true;
            }

            return packageDetails;
        }

        public static IPackageDetails Create(PackageOperationType operationType, IPackageSearchMetadata? versionMetadata, NuGetVersion? packageVersion, bool? isLastVersion)
        {
            ArgumentNullException.ThrowIfNull(versionMetadata);
            ArgumentNullException.ThrowIfNull(packageVersion);

            var selectedIdentity = new PackageIdentity(versionMetadata.Identity.Id, packageVersion);
            return Create(operationType, versionMetadata, selectedIdentity, isLastVersion);
        }

        /// <summary>
        /// Empty package details
        /// </summary>
        /// <param name="packageIdentity"></param>
        /// <returns></returns>
        public static IPackageDetails Create(PackageIdentity packageIdentity)
        {
            return new EmptyPackageDetails(packageIdentity);
        }
    }
}
