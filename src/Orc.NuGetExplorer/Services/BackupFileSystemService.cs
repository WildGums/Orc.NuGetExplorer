﻿namespace Orc.NuGetExplorer;

using System;
using System.IO;
using Catel.Logging;
using Orc.FileSystem;

internal class BackupFileSystemService : IBackupFileSystemService
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();
    private readonly IPackageOperationContextService _operationContextService;
    private readonly IDirectoryService _directoryService;
    private readonly IFileService _fileService;

    public BackupFileSystemService(IPackageOperationContextService operationContextService, IDirectoryService directoryService, IFileService fileService)
    {
        ArgumentNullException.ThrowIfNull(operationContextService);
        ArgumentNullException.ThrowIfNull(directoryService);
        ArgumentNullException.ThrowIfNull(fileService);

        _operationContextService = operationContextService;
        _directoryService = directoryService;
        _fileService = fileService;
    }

    public void BackupFolder(string fullPath)
    {
        Log.Info("Creating backup for {0}", fullPath);

        try
        {
            var destinationDirectory = GetBackupFolder(fullPath);

            _directoryService.Copy(fullPath, destinationDirectory);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create backup for {0}", fullPath);
        }
    }

    public void BackupFile(string filePath)
    {
        Log.Info("Creating backup for {0}", filePath);

        try
        {
            var fileName = Path.GetFileName(filePath);
            var fileDestinationPath = GetBackupFolder(Catel.IO.Path.GetDirectoryName(filePath));

            _fileService.Copy(filePath, Path.Combine(fileDestinationPath, fileName), true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create backup for {0}", filePath);
        }
    }

    public void Restore(string fullPath)
    {
        Log.Info("Restoring backup for {0}", fullPath);

        try
        {
            if (_fileService.Exists(fullPath))
            {
                //restore only single file on this path
                fullPath = Catel.IO.Path.GetDirectoryName(fullPath);
            }
            else
            {
                //clean-up directory from files created after backup, like .deleteme etc
                _directoryService.Delete(fullPath);
            }

            var sourceDirectory = GetBackupFolder(fullPath);

            _directoryService.Copy(sourceDirectory, fullPath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to restore backup for {0}", fullPath);
        }
    }

    private string GetBackupFolder(string fullPath)
    {
        var parentDirectory = Catel.IO.Path.GetParentDirectory(fullPath);
        var directoryName = Catel.IO.Path.GetRelativePath(fullPath, parentDirectory);
        var backupDirectory = _operationContextService.CurrentContext?.FileSystemContext.GetDirectory(directoryName);

        if (string.IsNullOrEmpty(backupDirectory))
        {
            throw Log.ErrorAndCreateException<InvalidPathException>($"Invalid path found for backup folder on '{fullPath}'");
        }

        return backupDirectory;
    }
}