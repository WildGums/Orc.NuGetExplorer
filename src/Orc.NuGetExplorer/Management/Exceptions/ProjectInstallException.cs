namespace Orc.NuGetExplorer.Management
{
    using System;
    using System.Collections.Generic;
    using NuGet.Packaging.Core;

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
