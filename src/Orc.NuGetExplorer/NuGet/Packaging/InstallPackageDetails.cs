namespace Orc.NuGetExplorer.Packaging
{
    using NuGet.Protocol.Core.Types;

    internal class InstallPackageDetails : PackageDetails
    {
        public InstallPackageDetails(IPackageSearchMetadata metadata,  bool isLatestVersion = false) : base(metadata, isLatestVersion)
        {
        }
    }
}
