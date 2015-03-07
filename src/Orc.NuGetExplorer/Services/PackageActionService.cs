// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageActionService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using Catel.Services;

    internal class PackageActionService : IPackageActionService
    {
        #region Fields
        private readonly INuGetPackageManager _packageManager;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public PackageActionService(IPleaseWaitService pleaseWaitService, INuGetPackageManager packageManager)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageManager);

            _pleaseWaitService = pleaseWaitService;
            _packageManager = packageManager;
        }
        #endregion

        #region Methods
        public string GetActionName(RepositoryCategoryType repositoryCategory)
        {
            switch (repositoryCategory)
            {
                case RepositoryCategoryType.Installed:
                    return "Uninstall";
                case RepositoryCategoryType.Online:
                    return "Install";
                case RepositoryCategoryType.Update:
                    return "Update";
            }

            return string.Empty;
        }

        public void Execute(RepositoryCategoryType repositoryCategory, PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            using (_pleaseWaitService.WaitingScope())
            {
                switch (repositoryCategory)
                {
                    case RepositoryCategoryType.Installed:
                        UninstallPackage(packageDetails);
                        break;
                    case RepositoryCategoryType.Online:
                        InstallPackage(packageDetails, allowedPrerelease);
                        break;
                    case RepositoryCategoryType.Update:
                        UpdatePackages(packageDetails, allowedPrerelease);
                        break;
                }
            }
        }

        public bool CanExecute(RepositoryCategoryType repositoryCategory, PackageDetails packageDetails)
        {
            return true;
        }

        public bool IsRefreshReqired(RepositoryCategoryType repositoryCategory)
        {
            switch (repositoryCategory)
            {
                case RepositoryCategoryType.Installed:
                    return true;
                case RepositoryCategoryType.Online:
                    return false;
                case RepositoryCategoryType.Update:
                    return true;
            }

            return false;
        }

        private void UninstallPackage(PackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            _packageManager.UninstallPackage(packageDetails.Package, true, false);
        }

        private void InstallPackage(PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            _packageManager.InstallPackage(packageDetails.Package, false, allowedPrerelease);
        }

        private void UpdatePackages(PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            _packageManager.UpdatePackage(packageDetails.Package, true, allowedPrerelease);
        }
        #endregion
    }
}