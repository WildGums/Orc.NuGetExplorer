// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManagerWatchService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Services;
    using NuGet;

    internal class PackageManagerWatchService : IPackageManagerWatchService
    {
        #region Fields
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageCacheService _packageCacheService;
        #endregion

        #region Constructors
        public PackageManagerWatchService(INuGetPackageManager nuGetPackageManager, IPackageCacheService packageCacheService,
            IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => nuGetPackageManager);
            Argument.IsNotNull(() => packageCacheService);
            Argument.IsNotNull(() => dispatcherService);

            _packageCacheService = packageCacheService;
            _dispatcherService = dispatcherService;

            nuGetPackageManager.PackageInstalling += OnPackageInstalling;
            nuGetPackageManager.PackageInstalled += OnPackageInstalled;
            nuGetPackageManager.PackageUninstalled += OnPackageUninstalled;
            nuGetPackageManager.PackageUninstalling += OnPackageUninstalling;
        }
        #endregion

        #region Methods
        public event EventHandler<NuGetPackageOperationEventArgs> PackageInstalling;
        public event EventHandler<NuGetPackageOperationEventArgs> PackageInstalled;
        public event EventHandler<NuGetPackageOperationEventArgs> PackageUninstalled;
        public event EventHandler<NuGetPackageOperationEventArgs> PackageUninstalling;

        private void OnPackageUninstalling(object sender, PackageOperationEventArgs e)
        {
            ReThrowEvent(PackageUninstalling, sender, e);
        }

        private void OnPackageUninstalled(object sender, PackageOperationEventArgs e)
        {
            ReThrowEvent(PackageUninstalled, sender, e);
        }

        private void OnPackageInstalled(object sender, PackageOperationEventArgs e)
        {
            ReThrowEvent(PackageInstalled, sender, e);
        }

        private void OnPackageInstalling(object sender, PackageOperationEventArgs e)
        {
            ReThrowEvent(PackageInstalling, sender, e);
        }

        private void ReThrowEvent(EventHandler<NuGetPackageOperationEventArgs> handler, object sender, PackageOperationEventArgs e)
        {
            Argument.IsNotNull(() => handler);

            _dispatcherService.Invoke(() =>
            {
                if (handler != null)
                {
                    var packageDetails = _packageCacheService.GetPackageDetails(e.Package);
                    handler(sender, new NuGetPackageOperationEventArgs(packageDetails, e.InstallPath));
                }
            });
        }
        #endregion
    }
}