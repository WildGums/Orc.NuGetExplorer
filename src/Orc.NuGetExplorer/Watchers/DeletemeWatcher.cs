namespace Orc.NuGetExplorer
{
    using Catel.Messaging;
    using Orc.FileSystem;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Messaging;

    public class DeletemeWatcher : PackageManagerWatcherBase
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IDirectoryService _directoryService;
        private readonly INuGetPackageManager _nuGetPackageManager;
        private readonly IExtensibleProject _defaultProject;
        public DeletemeWatcher(IPackageOperationNotificationService packageOperationNotificationService, IFileSystemService fileSystemService,
            IDirectoryService directoryService, INuGetPackageManager nuGetPackageManager, IDefaultExtensibleProjectProvider projectProvider, IMessageMediator messageMediator)
            : base(packageOperationNotificationService)
        {
            _fileSystemService = fileSystemService;
            _directoryService = directoryService;
            _nuGetPackageManager = nuGetPackageManager;

            messageMediator.Register<PackagingDeletemeMessage>(this, OnDeletemeMessageAsync);

            _defaultProject = projectProvider.GetDefaultProject();
        }
        private async void OnDeletemeMessageAsync(PackagingDeletemeMessage message)
        {
            if (message.Data.OperationType == PackageOperationType.Uninstall)
            {
                if (!_directoryService.Exists(message.Data.OperationPath))
                {
                    return;
                }

                _fileSystemService.CreateDeleteme(message.Data.Package.Id, message.Data.OperationPath);
            }

            if (message.Data.OperationType == PackageOperationType.Install)
            {
                _fileSystemService.RemoveDeleteme(message.Data.Package.Id, message.Data.OperationPath);

                //check is folder broken installation or not
                //this handle cases where we perform installation of version, previously not correctly removed
                if (await _nuGetPackageManager.IsPackageInstalledAsync(_defaultProject, message.Data.Package.GetIdentity(), default))
                {
                    return;
                }

                _fileSystemService.CreateDeleteme(message.Data.Package.Id, message.Data.OperationPath);
            }
        }
    }
}
