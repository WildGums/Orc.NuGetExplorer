namespace Orc.NuGetExplorer;

using System;

public class InvalidPathException : Exception
{
    public InvalidPathException()
    {
    }

    public InvalidPathException(string? message) 
        : base(message)
    {
    }

    public InvalidPathException(string? message, Exception? innerException) 
        : base(message, innerException)
    {
    }
}
