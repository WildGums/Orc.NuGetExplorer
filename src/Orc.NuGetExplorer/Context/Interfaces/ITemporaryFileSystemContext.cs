namespace Orc.NuGetExplorer
{
    using System;

    public interface ITemporaryFileSystemContext : IDisposable
    {
        #region Properties
        string RootDirectory { get; }
        #endregion

        #region Methods
        string GetDirectory(string relativeDirectoryName);
        string GetFile(string relativeFilePath);
        #endregion
    }
}
