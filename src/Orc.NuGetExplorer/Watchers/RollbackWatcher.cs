// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollbackWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Linq;
    using Catel;
    using Catel.Logging;

    public class RollbackWatcher : PackageManagerWatcherBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IBackupFileSystemService _backupFileSystemService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IRollbackPackageOperationService _rollbackPackageOperationService;
        #endregion

        #region Constructors
        public RollbackWatcher(IPackageOperationNotificationService packageOperationNotificationService, IPackageOperationContextService packageOperationContextService,
            IRollbackPackageOperationService rollbackPackageOperationService, IBackupFileSystemService backupFileSystemService, IFileSystemService fileSystemService)
            : base(packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => rollbackPackageOperationService);
            Argument.IsNotNull(() => backupFileSystemService);
            Argument.IsNotNull(() => fileSystemService);

            _packageOperationContextService = packageOperationContextService;
            _rollbackPackageOperationService = rollbackPackageOperationService;
            _backupFileSystemService = backupFileSystemService;
            _fileSystemService = fileSystemService;

            packageOperationContextService.OperationContextDisposing += OnOperationContextDisposing;
        }
        #endregion

        #region Methods
        private void OnOperationContextDisposing(object sender, OperationContextEventArgs e)
        {
            var context = e.PackageOperationContext;
            if (context.Exceptions.Any())
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
            var context = _packageOperationContextService.CurrentContext;
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
                    context
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
                    context
                );
            }

            base.OnOperationStarting(sender, e);
        }
        #endregion
    }
}
