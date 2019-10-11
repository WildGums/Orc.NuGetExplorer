namespace Orc.NuGetExplorer.Management.EventArgs
{
    extern alias NewNuGet;
    using NewNuGet::NuGet.Packaging.Core;

    public class NuGetProjectEventArgs : System.EventArgs
    {
        public NuGetProjectEventArgs(IExtensibleProject project, PackageIdentity package)
        {
            Project = project;
            Package = package;
        }

        public IExtensibleProject Project { get; }

        public PackageIdentity Package { get; }
    }
}
