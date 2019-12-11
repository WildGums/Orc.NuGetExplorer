// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeletemeWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.IO;
    using Catel;

    public class DeletemeWatcher : PackageManagerWatcherBase
    {
        private readonly IFileSystemService _fileSystemService;
        #region Constructors
        public DeletemeWatcher(IPackageOperationNotificationService packageOperationNotificationService, IFileSystemService fileSystemService) : base(packageOperationNotificationService)
        {
            Argument.IsNotNull(() => fileSystemService);

            _fileSystemService = fileSystemService;
        }
        #endregion

        #region Methods
        protected override void OnOperationFinished(object sender, PackageOperationEventArgs e)
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
                //handle cases where we reinstall package not removed correctly
                _fileSystemService.RemoveDeleteme(e.PackageDetails.Id, e.InstallPath);
            }
        }
        #endregion
    }
}
