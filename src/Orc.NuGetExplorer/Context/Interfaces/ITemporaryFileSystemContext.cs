namespace Orc.NuGetExplorer;

using System;

public interface ITemporaryFileSystemContext : IDisposable
{
    string RootDirectory { get; }

    string GetDirectory(string relativeDirectoryName);
    string GetFile(string relativeFilePath);
}