// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManagerListenerBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    public abstract class PackageManagerListenerBase
    {
        #region Constructors
        public PackageManagerListenerBase(IPackageManagerListeningService packageManagerListeningService)
        {
            Argument.IsNotNull(() => packageManagerListeningService);

            packageManagerListeningService.PackageInstalled += OnPackageInstalled;
            packageManagerListeningService.PackageInstalling += OnPackageInstalling;
            packageManagerListeningService.PackageUninstalled += OnPackageUninstalled;
            packageManagerListeningService.PackageUninstalling += OnPackageUninstalling;
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