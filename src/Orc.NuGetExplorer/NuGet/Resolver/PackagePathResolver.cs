namespace Orc.NuGetExplorer.Resolver
{
    using System.Collections.Generic;
    using NuGet.Versioning;

    public class PackagePathResolver : NuGet.Packaging.PackagePathResolver
    {
        public PackagePathResolver(IExtensibleProject extensibleProject, bool useSideBySidePaths = true)
            : this(extensibleProject.ContentPath, useSideBySidePaths)
        {

        }

        public PackagePathResolver(string rootDirectory, bool useSideBySidePaths = true)
            : base(rootDirectory, useSideBySidePaths)
        {
        }

        public virtual Dictionary<NuGetVersion, string> GetInstalledPackageFilePaths(string packageId, VersionRange versionRange)
        {
            var pathCollection = new Dictionary<NuGetVersion, string>();
            return pathCollection;
        }
    }
}
