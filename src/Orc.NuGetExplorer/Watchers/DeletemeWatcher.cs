// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeletemeWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.IO;
    using Catel;
    using Orc.NuGetExplorer.Management;

    public class DeletemeWatcher : PackageManagerWatcherBase
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly INuGetPackageManager _nuGetPackageManager;
        private readonly IExtensibleProject _defaultProject;
        #region Constructors
        public DeletemeWatcher(IPackageOperationNotificationService packageOperationNotificationService, IFileSystemService fileSystemService,
            INuGetPackageManager nuGetPackageManager, IDefaultExtensibleProjectProvider projectProvider) : base(packageOperationNotificationService)
        {
            Argument.IsNotNull(() => fileSystemService);
            Argument.IsNotNull(() => nuGetPackageManager);
            Argument.IsNotNull(() => projectProvider);

            _fileSystemService = fileSystemService;
            _nuGetPackageManager = nuGetPackageManager;
            _defaultProject = projectProvider.GetDefaultProject();
        }
        #endregion

        #region Methods
        protected override async void OnOperationFinished(object sender, PackageOperationEventArgs e)
        {
            if (e.PackageOperationType == PackageOperationType.Uninstall)
            {
                if (!Directory.Exists(e.InstallPath))
                {
                    return;
                }

                _fileSystemService.CreateDeleteme(e.PackageDetails.Id, e.InstallPath);
            }

            if (e.PackageOperationType == PackageOperationType.Install)
            {
                _fileSystemService.RemoveDeleteme(e.PackageDetails.Id, e.InstallPath);

                //check is folder broken installation or not
                //this handle cases where we perform installation of version, previously not correctly removed
                if (await _nuGetPackageManager.IsPackageInstalledAsync(_defaultProject, e.PackageDetails.GetIdentity(), default))
                {
                    return;
                }

                _fileSystemService.CreateDeleteme(e.PackageDetails.Id, e.InstallPath);
            }
        }

        #endregion
    }
}
