namespace Orc.NuGetExplorer.Management
{
    using System.Collections.Generic;
    using System.Linq;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;

    /// <summary>
    /// Default project which represents "plugins" folder
    /// </summary>
    public class DestFolder : IExtensibleProject
    {
        private readonly PackagePathResolver _pathResolver;

        public DestFolder(string destinationFolder, IDefaultNuGetFramework defaultFramework)
        {
            ContentPath = destinationFolder;

            var lowest = defaultFramework.GetLowest();

            Framework = lowest.LastOrDefault()?.ToString();

            _pathResolver = new PackagePathResolver(ContentPath);
        }

        public string Name => "Plugins";

        public string Framework { get; private set; }

        public IEnumerable<NuGetFramework> SupportedFrameworks { get; set; }

        public string ContentPath { get; private set; }

        public string GetInstallPath(PackageIdentity packageIdentity)
        {
            return _pathResolver.GetInstallPath(packageIdentity);
        }

        public void Install()
        {

        }

        public void Uninstall()
        {

        }

        public void Update()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
