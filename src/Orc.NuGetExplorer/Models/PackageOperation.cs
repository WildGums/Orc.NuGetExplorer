namespace Orc.NuGetExplorer
{
    using System.Diagnostics;
    using NuGet.PackageManagement;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;

    /// <summary>
    /// Represents an atomic INuGetPackageManager operation, that use own type of Project
    /// </summary>
    [DebuggerDisplay("{NuGetProjectActionType} {PackageIdentity}")]
    public class PackageOperation : NuGetProjectAction
    {
        public static PackageOperation CreateInstallPackageOperation(PackageIdentity packageIdentity, SourceRepository sourceRepository, IExtensibleProject project)
        {
            return new PackageOperation(packageIdentity, NuGetProjectActionType.Install, project, sourceRepository);
        }

        public static PackageOperation CreateUninstallPackageOperation(PackageIdentity packageIdentity, IExtensibleProject project)
        {
            return new PackageOperation(packageIdentity, NuGetProjectActionType.Uninstall, project, null);
        }

        protected PackageOperation(PackageIdentity packageIdentity, NuGetProjectActionType nuGetProjectActionType, IExtensibleProject project, SourceRepository sourceRepository = null)
            : base(packageIdentity, nuGetProjectActionType, null, sourceRepository)
        {
            Project = project;
        }

        public new IExtensibleProject Project { get; private set; }
    }

    /// <summary>
    /// The combination of delete/install operations
    /// </summary>
    public class UpdatePackageOperation : PackageOperation
    {
        public static PackageOperation CreateUpdatePackageOperation(PackageOperation install, PackageOperation uninstall, IExtensibleProject project)
        {
            var operation = new UpdatePackageOperation(install.PackageIdentity, NuGetProjectActionType.Install, project, install.SourceRepository)
            {
                Install = install,
                Uninstall = uninstall
            };

            return operation;
        }

        protected UpdatePackageOperation(PackageIdentity packageIdentity, NuGetProjectActionType nuGetProjectActionType, IExtensibleProject project, SourceRepository sourceRepository = null)
            : base(packageIdentity, nuGetProjectActionType, project, sourceRepository)
        {
        }

        public PackageOperation Install { get; private set; }

        public PackageOperation Uninstall { get; private set; }
    }
}
