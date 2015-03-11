// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeletemeListener.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.IO;
    using Path = Catel.IO.Path;

    public class DeletemeListener : PackageManagementListenerBase
    {
        #region Constructors
        public DeletemeListener(IPackageManagementListeningService packageManagementListeningService) : base(packageManagementListeningService)
        {
        }
        #endregion

        #region Methods
        protected override void OnPackageUninstalled(object sender, NuGetPackageOperationEventArgs e)
        {
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