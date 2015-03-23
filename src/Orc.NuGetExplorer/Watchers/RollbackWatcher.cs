// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollbackWatcher.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Linq;
    using Catel;

    internal class RollbackWatcher : PackageManagerWatcherBase
    {
        #region Fields
        private readonly IBackupFileSystemService _backupFileSystemService;
        private readonly IFIleSystemService _fIleSystemService;
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IRollbackPackageOperationService _rollbackPackageOperationService;
        #endregion

        #region Constructors
        public RollbackWatcher(IPackageOperationNotificationService packageOperationNotificationService, IPackageOperationContextService packageOperationContextService,
            IRollbackPackageOperationService rollbackPackageOperationService, IBackupFileSystemService backupFileSystemService, IFIleSystemService fIleSystemService)
            : base(packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => rollbackPackageOperationService);
            Argument.IsNotNull(() => backupFileSystemService);
            Argument.IsNotNull(() => fIleSystemService);

            _packageOperationContextService = packageOperationContextService;
            _rollbackPackageOperationService = rollbackPackageOperationService;
            _backupFileSystemService = backupFileSystemService;
            _fIleSystemService = fIleSystemService;

            packageOperationContextService.OperationContextDisposing += OnOperationContextDisposing;
        }

        private void OnOperationContextDisposing(object sender, OperationContextEventArgs e)
        {
            var context = e.PackageOperationContext;
            if (context.CatchedExceptions.Any())
            {
                _rollbackPackageOperationService.Rollback(context);
            }
            else
            {
                _rollbackPackageOperationService.ClearRollbackActions(context);
            }
        }
        #endregion

        #region Methods
        protected override void OnOperationStarting(object sender, PackageOperationEventArgs e)
        {
            var context = _packageOperationContextService.CurrentContext;
            if (e.PackageOperationType == PackageOperationType.Uninstall)
            {
                _backupFileSystemService.BackupFolder(e.InstallPath);
                _rollbackPackageOperationService.PushRollbackAction(() => _backupFileSystemService.Restore(e.InstallPath), context);
                return;
            }

            if (e.PackageOperationType == PackageOperationType.Install)
            {
                _rollbackPackageOperationService.PushRollbackAction(() => _fIleSystemService.DeleteDirectory(e.InstallPath), context);
            }

            base.OnOperationStarting(sender, e);
        }


        #endregion
    }
}