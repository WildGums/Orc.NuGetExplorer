namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Logging;
    using Orc.FileSystem;

    public class RollbackWatcher : PackageManagerContextWatcherBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IBackupFileSystemService _backupFileSystemService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IDirectoryService _directoryService;
        private readonly IRollbackPackageOperationService _rollbackPackageOperationService;

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

        protected override void OnOperationContextDisposing(object? sender, OperationContextEventArgs e)
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

        protected override void OnOperationStarting(object? sender, PackageOperationEventArgs e)
        {
            var packagesConfig = System.IO.Path.Combine(Catel.IO.Path.GetParentDirectory(e.InstallPath), "packages.config");

            if (e.PackageOperationType == PackageOperationType.Uninstall)
            {
                _backupFileSystemService.BackupFolder(e.InstallPath);
                _backupFileSystemService.BackupFile(packagesConfig);

                _rollbackPackageOperationService.PushRollbackAction(() =>
                {
                    _backupFileSystemService.Restore(e.InstallPath);
                    _backupFileSystemService.Restore(packagesConfig);
                }, CurrentContext);
            }

            if (e.PackageOperationType == PackageOperationType.Install)
            {
                _rollbackPackageOperationService.PushRollbackAction(() =>
                {
                    var success = true;
                    try
                    {
                        _directoryService.Delete(e.InstallPath);
                        success = !_directoryService.Exists(e.InstallPath);
                    }
                    catch (Exception)
                    {
                        success = false;
                    }
                    finally
                    {
                        if (!success)
                        {
                            _fileSystemService.CreateDeleteme(e.PackageDetails.Id, e.InstallPath);
                            Log.Error($"Failed to delete directory {e.InstallPath} during rollback actions.");
                        }
                    }
                },
                    CurrentContext
                );
            }
        }
    }
}
