namespace Orc.NuGetExplorer.Management.EventArgs
{
    extern alias NewNuGet;
    using NewNuGet::NuGet.Packaging.Core;

    public class UninstallNuGetProjectEventArgs : NuGetProjectEventArgs
    {
        public UninstallNuGetProjectEventArgs(IExtensibleProject project, PackageIdentity package, bool result) : base(project, package)
        {
            Result = result;
        }

        public bool Result { get; }
    }

    public class BatchedUninstallNuGetProjectEventArgs : UninstallNuGetProjectEventArgs
    {
        public BatchedUninstallNuGetProjectEventArgs(UninstallNuGetProjectEventArgs eventArgs) : base(eventArgs.Project, eventArgs.Package, eventArgs.Result)
        {
        }

        public bool IsBatchEnd { get; set; }
    }
}
