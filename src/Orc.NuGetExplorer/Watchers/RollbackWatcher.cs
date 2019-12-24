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

    public class RollbackWatcher : PackageManagerContextWatcherBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IBackupFileSystemService _backupFileSystemService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IRollbackPackageOperationService _rollbackPackageOperationService;
        #endregion

        #region Constructors
        public RollbackWatcher(IPackageOperationNotificationService packageOperationNotificationService, IPackageOperationContextService packageOperationContextService,
            IRollbackPackageOperationService rollbackPackageOperationService, IBackupFileSystemService backupFileSystemService, IFileSystemService fileSystemService)
            : base(packageOperationNotificationService, packageOperationContextService)
        {
            Argument.IsNotNull(() => rollbackPackageOperationService);
            Argument.IsNotNull(() => backupFileSystemService);
            Argument.IsNotNull(() => fileSystemService);

            _rollbackPackageOperationService = rollbackPackageOperationService;
            _backupFileSystemService = backupFileSystemService;
            _fileSystemService = fileSystemService;
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
            var packagesConfig = Catel.IO.Path.Combine(Catel.IO.Path.GetParentDirectory(e.InstallPath), "packages.config");

            if (e.PackageOperationType == PackageOperationType.Uninstall)
            {
                _backupFileSystemService.BackupFolder(e.InstallPath);
                _backupFileSystemService.BackupFile(packagesConfig);
                _rollbackPackageOperationService.PushRollbackAction(() =>
                    {
                        _backupFileSystemService.Restore(e.InstallPath);
                        _backupFileSystemService.Restore(packagesConfig);
                    },
                    CurrentContext
                );
                return;
            }

            if (e.PackageOperationType == PackageOperationType.Install)
            {
                _rollbackPackageOperationService.PushRollbackAction(() =>
                    {
                        try
                        {
                            _fileSystemService.DeleteDirectory(e.InstallPath);
                        }
                        catch (Exception ex)
                        {
                            _fileSystemService.CreateDeleteme(e.PackageDetails.Id, e.InstallPath);
                            Log.Error("Failed to delete directory during rollback actions", ex);
                        }
                    },
                    CurrentContext
                );
            }

            base.OnOperationStarting(sender, e);
        }
        #endregion
    }
}
