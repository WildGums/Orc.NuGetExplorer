// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeletemeWatcher.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.IO;
    using Path = Catel.IO.Path;

    public class DeletemeWatcher : PackageManagerWatcherBase
    {
        #region Constructors
        public DeletemeWatcher(IPackageOperationNotificationService packageOperationNotificationService) : base(packageOperationNotificationService)
        {
        }
        #endregion

        #region Methods
        protected override void OnOperationFinished(object sender, PackageOperationEventArgs e)
        {
            if (e.PackageOperationType != PackageOperationType.Uninstall)
            {
                return;
            }

            if (!Directory.Exists(e.InstallPath))
            {
                return;
            }

            var fileName = $"{e.PackageDetails.Id}.deleteme";
            var fullName = Path.Combine(e.InstallPath, fileName);

            using (File.Create(fullName))
            {
            }
        }
        #endregion
    }
}
