﻿namespace Orc.NuGetExplorer;

using System;
using System.IO;
using Catel.Logging;
using Catel.Reflection;
using FileSystem;

internal class TemporaryFileSystemContext : ITemporaryFileSystemContext
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IDirectoryService _directoryService;
    private readonly string _rootDirectory;

    public TemporaryFileSystemContext(IDirectoryService directoryService)
    {
        ArgumentNullException.ThrowIfNull(directoryService);

        _directoryService = directoryService;

        var assembly = AssemblyHelper.GetRequiredEntryAssembly();

        _rootDirectory = Path.Combine(Path.GetTempPath(), assembly.Company() ?? string.Empty, assembly.Title() ?? string.Empty,
            "backup", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

        _directoryService.Create(_rootDirectory);
    }

    public string RootDirectory
    {
        get { return _rootDirectory; }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
#pragma warning disable IDISP023 // Don't use reference types in finalizer context.
        try
        {
            Log.Info("Deleting temporary files from '{0}'", _rootDirectory);

            _directoryService.Delete(_rootDirectory, true);

            Log.Info("Temporary files has been successfully deleted from '{0}'", _rootDirectory);
        }
        catch (Exception)
        {
            Log.Warning("Unable to cleanup temporary files");
        }
#pragma warning restore IDISP023 // Don't use reference types in finalizer context.
    }

    public string GetDirectory(string relativeDirectoryName)
    {
        var fullPath = Path.Combine(_rootDirectory, relativeDirectoryName);

        if (!_directoryService.Exists(fullPath))
        {
            _directoryService.Create(fullPath);
        }

        return fullPath;
    }

    public string GetFile(string relativeFilePath)
    {
        var fullPath = Path.Combine(_rootDirectory, relativeFilePath);
        var directory = Path.GetDirectoryName(fullPath) ?? string.Empty;

        if (!_directoryService.Exists(directory))
        {
            _directoryService.Create(directory);
        }

        return fullPath;
    }
}
