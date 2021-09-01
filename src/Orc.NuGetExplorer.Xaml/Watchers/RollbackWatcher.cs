// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollbackWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Logging;
    using NuGet.PackageManagement;
    using Orc.FileSystem;

    public class RollbackWatcher : PackageManagerContextWatcherBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IBackupFileSystemService _backupFileSystemService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IDirectoryService _directoryService;
        private readonly IRollbackPackageOperationService _rollbackPackageOperationService;
        #endregion

        #region Constructors
        public RollbackWatcher(IPackageOperationNotificationService packageOperationNotificationService, IPackageOperationContextService packageOperationContextService,
            IRollbackPackageOperationService rollbackPackageOperationService, IBackupFileSystemService backupFileSystemService, IFileSystemService fileSystemService,
            IDirectoryService directoryService)
            : base(packageOperationNotificationService, packageOperationContextService)
        {
            Argument.IsNotNull(() => rollbackPackageOperationService);
            Argument.IsNotNull(() => backupFileSystemService);
            Argument.IsNotNull(() => fileSystemService);
            Argument.IsNotNull(() => directoryService);

            _rollbackPackageOperationService = rollbackPackageOperationService;
            _backupFileSystemService = backupFileSystemService;
            _fileSystemService = fileSystemService;
            _directoryService = directoryService;
        }
        #endregion

        #region Methods
        protected override void OnOperationContextDisposing(object sender, OperationContextEventArgs e)
        {
            var context = e.PackageOperationContext;

            if (HasContextErrors)
            {
                _rollbackPackageOperationService.Rollback(context);
            }
            else
            {
                _rollbackPackageOperationService.ClearRollbackActions(context);
            }
        }

        protected override void OnOperationStarting(object sender, PackageOperationEventArgs e)
        {
            var package = e.Details.PackageIdentity;
            var project = e.Details.Project;
            var operationType = e.Details.NuGetProjectActionType;
            var installPath = project.GetInstallPath(package);
            var packagesConfig = Catel.IO.Path.Combine(Catel.IO.Path.GetParentDirectory(installPath), "packages.config");

            // TODO: warning - test this part with unit tests/integration to be sure rollback shouldn't do any on update
            if (e.IsUpdate)
            {
                return;
            }

            if (operationType == NuGetProjectActionType.Uninstall)
            {
                _backupFileSystemService.BackupFolder(installPath);
                _backupFileSystemService.BackupFile(packagesConfig);

                _rollbackPackageOperationService.PushRollbackAction(() =>
                {
                    _backupFileSystemService.Restore(installPath);
                    _backupFileSystemService.Restore(packagesConfig);
                }, CurrentContext);
            }

            if (operationType == NuGetProjectActionType.Install)
            {
                _rollbackPackageOperationService.PushRollbackAction(() =>
                {
                    bool success = true;
                    try
                    {
                        _directoryService.Delete(installPath);
                        success = !_directoryService.Exists(installPath);
                    }
                    catch (Exception)
                    {
                        success = false;
                    }
                    finally
                    {
                        if (!success)
                        {
                            _fileSystemService.CreateDeleteme(package.Id, installPath);
                            Log.Error($"Failed to delete directory {installPath} during rollback actions.");
                        }
                    }
                },
                    CurrentContext
                );
            }
        }
        #endregion
    }
}
