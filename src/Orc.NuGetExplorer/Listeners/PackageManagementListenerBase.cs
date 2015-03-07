// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManagementListenerBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Listeners
{
    using Catel;

    public abstract class PackageManagementListenerBase
    {
        #region Constructors
        public PackageManagementListenerBase(IPackageManagementListeningService packageManagementListeningService)
        {
            Argument.IsNotNull(() => packageManagementListeningService);

            packageManagementListeningService.PackageInstalled += OnPackageInstalled;
            packageManagementListeningService.PackageInstalling += OnPackageInstalling;
            packageManagementListeningService.PackageUninstalled += OnPackageUninstalled;
            packageManagementListeningService.PackageUninstalling += OnPackageUninstalling;
        }
        #endregion

        #region Methods
        protected virtual void OnPackageInstalled(object sender, NuGetPackageOperationEventArgs e)
        {
        }

        protected virtual void OnPackageUninstalling(object sender, NuGetPackageOperationEventArgs e)
        {
        }

        protected virtual void OnPackageUninstalled(object sender, NuGetPackageOperationEventArgs e)
        {
        }

        protected virtual void OnPackageInstalling(object sender, NuGetPackageOperationEventArgs e)
        {
        }
        #endregion
    }
}