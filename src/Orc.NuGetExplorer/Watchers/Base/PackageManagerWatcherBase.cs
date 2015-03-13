// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManagerWatcherBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    public abstract class PackageManagerWatcherBase
    {
        #region Constructors
        public PackageManagerWatcherBase(IPackageManagerWatchService packageManagerWatchService)
        {
            Argument.IsNotNull(() => packageManagerWatchService);

            packageManagerWatchService.PackageInstalled += OnPackageInstalled;
            packageManagerWatchService.PackageInstalling += OnPackageInstalling;
            packageManagerWatchService.PackageUninstalled += OnPackageUninstalled;
            packageManagerWatchService.PackageUninstalling += OnPackageUninstalling;
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