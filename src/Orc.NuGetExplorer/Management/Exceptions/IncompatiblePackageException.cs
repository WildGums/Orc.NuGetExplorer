namespace Orc.NuGetExplorer.Management.Exceptions;

using System;

public class IncompatiblePackageException : ProjectInstallException
{
    public IncompatiblePackageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public IncompatiblePackageException(string message) : base(message)
    {
    }
}