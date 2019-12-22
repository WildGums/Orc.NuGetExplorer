// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackupFileSystemService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Logging;

    internal class BackupFileSystemService : IBackupFileSystemService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IFileSystemService _fileSystemService;
        private readonly IPackageOperationContextService _operationContextService;
        #endregion

        #region Constructors
        public BackupFileSystemService(IPackageOperationContextService operationContextService, IFileSystemService fileSystemService)
        {
            Argument.IsNotNull(() => operationContextService);
            Argument.IsNotNull(() => fileSystemService);

            _operationContextService = operationContextService;
            _fileSystemService = fileSystemService;
        }
        #endregion

        #region Methods
        public void BackupFolder(string fullPath)
        {
            Log.Info("Creating backup for {0}", fullPath);

            try
            {
                var destinationDirectory = GetBackupFolder(fullPath);

                _fileSystemService.CopyDirectory(fullPath, destinationDirectory);
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

                File.Copy(filePath, Catel.IO.Path.Combine(fileDestinationPath, fileName), true);
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
                if (File.Exists(fullPath))
                {
                    //restore only single file on this path
                    fullPath = Catel.IO.Path.GetDirectoryName(fullPath);
                }
                else
                {
                    //clean-up directory from files created after backup, like .deleteme etc
                    _fileSystemService.DeleteDirectory(fullPath);
                }

                var sourceDirectory = GetBackupFolder(fullPath);
                _fileSystemService.CopyDirectory(sourceDirectory, fullPath);
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
            var backupDirectory = _operationContextService.CurrentContext.FileSystemContext.GetDirectory(directoryName);

            return backupDirectory;
        }

        #endregion
    }
}
