// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageActionService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Services;
    using NuGet;

    internal class PackageActionService : IPackageActionService
    {
        #region Fields
        private readonly INuGetPackageManager _packageManager;
        private readonly IPackageRepositoryService _packageRepositoryService;
        private readonly ILogger _logger;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public PackageActionService(IPleaseWaitService pleaseWaitService, INuGetPackageManager packageManager,
            IPackageRepositoryService packageRepositoryService, ILogger logger)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageManager);
            Argument.IsNotNull(() => packageRepositoryService);
            Argument.IsNotNull(() => logger);

            _pleaseWaitService = pleaseWaitService;
            _packageManager = packageManager;
            _packageRepositoryService = packageRepositoryService;
            _logger = logger;
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

        public void Execute(RepositoryCategoryType repositoryCategory, IPackageRepository remoteRepository, 
            PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            using (_pleaseWaitService.WaitingScope())
            {
                switch (repositoryCategory)
                {
                    case RepositoryCategoryType.Installed:
                        UninstallPackage(remoteRepository, packageDetails);
                        break;
                    case RepositoryCategoryType.Online:
                        InstallPackage(remoteRepository, packageDetails, allowedPrerelease);
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

        private void UninstallPackage(IPackageRepository remoteRepository, PackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            try
            {
                var localRepository = _packageRepositoryService.LocalRepository;
                var walker = new UninstallWalker(localRepository, new DependentsWalker(remoteRepository, null), null, 
                    _logger, false, false);

                var operations = walker.ResolveOperations(packageDetails.Package);
                foreach (var operation in operations)
                {
                    ExecuteOperation(true, operation);
                }
            }
            catch(Exception exception)
            {
                _logger.Log(MessageLevel.Error, exception.ToString());
            }            

            _packageManager.UninstallPackage(packageDetails.Package, true, false);
        }

        private void InstallPackage(IPackageRepository remoteRepository, PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => remoteRepository);
            Argument.IsNotNull(() => packageDetails);

            try
            {
                var localRepository = _packageRepositoryService.LocalRepository;
                var walker = new InstallWalker(localRepository, remoteRepository, null, _logger, false, allowedPrerelease, 
                    DependencyVersion.Lowest);

                var operations = walker.ResolveOperations(packageDetails.Package);
                foreach (var operation in operations)
                {
                    ExecuteOperation(allowedPrerelease, operation);
                }
            }
            catch(Exception exception)
            {
                _logger.Log(MessageLevel.Error, exception.ToString());
            }
        }

        private void ExecuteOperation(bool allowedPrerelease, PackageOperation operation)
        {
            switch (operation.Action)
            {
                case PackageAction.Install:
                    _packageManager.InstallPackage(operation.Package, false, allowedPrerelease);
                    break;
                case PackageAction.Uninstall:
                    _packageManager.UpdatePackage(operation.Package, true, allowedPrerelease);
                    break;
            }
        }

        private void UpdatePackages(PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            _packageManager.UpdatePackage(packageDetails.Package, true, allowedPrerelease);
        }
        #endregion
    }
}