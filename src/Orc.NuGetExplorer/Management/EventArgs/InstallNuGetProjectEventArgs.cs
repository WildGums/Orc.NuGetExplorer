namespace Orc.NuGetExplorer.Management;

using NuGet.Packaging.Core;

public class InstallNuGetProjectEventArgs : NuGetProjectEventArgs
{
    public InstallNuGetProjectEventArgs(IExtensibleProject project, PackageIdentity package, bool result) : base(project, package)
    {
        Result = result;
    }

    public bool Result { get; }
}

public class BatchedInstallNuGetProjectEventArgs : InstallNuGetProjectEventArgs
{
    public BatchedInstallNuGetProjectEventArgs(InstallNuGetProjectEventArgs eventArgs) : base(eventArgs.Project, eventArgs.Package, eventArgs.Result)
    {
    }

    public bool IsBatchEnd { get; set; }
}