namespace Orc.NuGetExplorer.Management.Exceptions
{
    using System;

    public class MissedPackageException : ProjectInstallException
    {
        public MissedPackageException(string message) : base(message)
        {
        }

        public MissedPackageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
