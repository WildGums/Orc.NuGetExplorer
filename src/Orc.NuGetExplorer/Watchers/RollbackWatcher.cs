// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollbackWatcher.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
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
        }
        #endregion

        #region Methods
        protected override void OnOperationStarting(object sender, PackageOperationEventArgs e)
        {
            if (e.PackageOperationType == PackageOperationType.Uninstall)
            {
                _backupFileSystemService.BackupFolder(e.InstallPath);
                _rollbackPackageOperationService.PushRollbackAction(() => _backupFileSystemService.Restore(e.InstallPath));
                return;
            }

            if (e.PackageOperationType == PackageOperationType.Install)
            {
                _rollbackPackageOperationService.PushRollbackAction(() => _fIleSystemService.DeleteDirectory(e.InstallPath));
            }

            base.OnOperationStarting(sender, e);
        }
        #endregion
    }
}