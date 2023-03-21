namespace Orc.NuGetExplorer.Management;

using System;

public class ProjectStateException : Exception
{
    public ProjectStateException(string message)
        : base(message)
    {
    }
}