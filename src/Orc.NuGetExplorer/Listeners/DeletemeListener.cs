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
        public DeletemeListener(IPackageManagementListeningService packageManagementListeningService) : base(packageManagementListeningService)
        {
        }

        protected override void OnPackageUninstalling(object sender, NuGetPackageOperationEventArgs e)
        {
            var fileName = string.Format("{0}.deleteme", e.PackageDetails.Id);
            var fullName = Path.Combine(e.InstallPath, fileName);

            using(File.Create(fullName))
            { }
        }
    }
}