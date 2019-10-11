// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackupFileSystemService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System;
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
                var parentDirectory = Catel.IO.Path.GetParentDirectory(fullPath);
                var directoryName = Catel.IO.Path.GetRelativePath(fullPath, parentDirectory);
                var destinationDirectory = _operationContextService.CurrentContext.FileSystemContext.GetDirectory(directoryName);

                _fileSystemService.CopyDirectory(fullPath, destinationDirectory);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to create backup for {0}", fullPath);
            }
        }

        public void Restore(string fullPath)
        {
            Log.Info("Restoring backup for {0}", fullPath);

            try
            {
                var parentDirectory = Catel.IO.Path.GetParentDirectory(fullPath);
                var directoryName = Catel.IO.Path.GetRelativePath(fullPath, parentDirectory);
                var sourceDirectory = _operationContextService.CurrentContext.FileSystemContext.GetDirectory(directoryName);

                _fileSystemService.CopyDirectory(sourceDirectory, fullPath);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to restore backup for {0}", fullPath);
            }
        }
        #endregion
    }
}