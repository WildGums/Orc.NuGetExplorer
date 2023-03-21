namespace Orc.NuGetExplorer;

using System;

internal interface ITemporaryFIleSystemContextService
{
    ITemporaryFileSystemContext? Context { get; }

    IDisposable UseTemporaryFIleSystemContext();
}