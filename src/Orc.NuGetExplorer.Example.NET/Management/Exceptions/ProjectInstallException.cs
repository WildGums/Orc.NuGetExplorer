namespace Orc.NuGetExplorer.Management
{
    extern alias NewNuGet;
    using NewNuGet::NuGet.Packaging.Core;
    using System;
    using System.Collections.Generic;

    public class ProjectInstallException : ProjectManageException
    {

        public ProjectInstallException(string message) : base(message)
        {

        }

        public ProjectInstallException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public IEnumerable<PackageIdentity> CurrentBatch { get; set; }
    }
}
