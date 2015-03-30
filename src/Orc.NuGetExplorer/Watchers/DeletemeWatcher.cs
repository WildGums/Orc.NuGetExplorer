// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeletemeWatcher.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.IO;
    using Catel;
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

            var fileName = string.Format("{0}.deleteme", e.PackageDetails.Id);
            var fullName = Path.Combine(e.InstallPath, fileName);

            using (File.Create(fullName))
            {
            }
        }
        #endregion
    }
}