namespace Orc.NuGetExplorer.Management
{
    using System.Collections.Immutable;
    using Catel.Logging;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;

    public class ExampleFolderPackageManagement : IExtensibleProject
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly PackagePathResolver _pathResolver;

        public ExampleFolderPackageManagement(string rootPath)
        {
            ContentPath = System.IO.Path.Combine(rootPath, nameof(ExampleFolderPackageManagement));

            _pathResolver = new PackagePathResolver(ContentPath);
        }

        public string Name => "Plain project extensible example with additinal logging";

        public string Framework => ".NETStandard,Version=v2.1";

        public string ContentPath { get; }

        public ImmutableList<NuGetFramework> SupportedPlatforms { get; set; } = ImmutableList.Create<NuGetFramework>();

        public string GetInstallPath(PackageIdentity packageIdentity)
        {
            return _pathResolver.GetInstallPath(packageIdentity);
        }

        public void Install()
        {
            Log.Info("Installation started");
        }

        public void Uninstall()
        {
            Log.Info("Uninstall started");
        }

        public void Update()
        {
            Log.Info("Update started");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
