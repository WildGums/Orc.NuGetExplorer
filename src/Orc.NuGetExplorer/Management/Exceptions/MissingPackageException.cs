namespace Orc.NuGetExplorer.Management.Exceptions
{
    using System;

    public class MissingPackageException : ProjectInstallException
    {
        public MissingPackageException(string message) : base(message)
        {
        }

        public MissingPackageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
