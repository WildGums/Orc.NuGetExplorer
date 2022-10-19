namespace Orc.NuGetExplorer.Management
{
    using System;

    public class ProjectManageException : Exception
    {
        public ProjectManageException(string message) : base(message)
        {
        }

        public ProjectManageException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
