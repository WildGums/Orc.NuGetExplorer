// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageActionService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using NuGet;

    internal class PackageActionService : IPackageActionService
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly INuGetPackageManager _packageManager;
        private readonly IPackageRepositoryService _packageRepositoryService;
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly IPackageRepository _localRepository;
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
            _localRepository = _packageRepositoryService.LocalRepository;
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

        public async Task Execute(RepositoryCategoryType repositoryCategory, IPackageRepository remoteRepository, PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            await Task.Factory.StartNew(() =>
            {
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
                            UpdatePackages(remoteRepository, packageDetails, allowedPrerelease);
                            break;
                    }
                }
            });
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

            var dependentsResolver = new DependentsWalker(remoteRepository, null);

            var walker = new UninstallWalker(_localRepository, dependentsResolver, null,
                _logger, false, false);

            ExecuteOperation(packageDetails, walker);

            _packageManager.UninstallPackage(packageDetails.Package, true, false);
        }

        private void InstallPackage(IPackageRepository remoteRepository, PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => remoteRepository);
            Argument.IsNotNull(() => packageDetails);

           

            var walker = new InstallWalker(_localRepository, remoteRepository, null, _logger, false, allowedPrerelease,
                DependencyVersion.Highest);

            ExecuteOperation(packageDetails, walker, allowedPrerelease);
        }

        private void UpdatePackages(IPackageRepository remoteRepository, PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            var dependentsResolver = new DependentsWalker(remoteRepository, null);

            var walker = new UpdateWalker(_localRepository, remoteRepository, dependentsResolver, null, null, _logger, true, allowedPrerelease);

            ExecuteOperation(packageDetails, walker, allowedPrerelease);
            // _packageManager.UpdatePackage(packageDetails.Package, true, allowedPrerelease);
        }

        private void ExecuteOperation(PackageDetails packageDetails, IPackageOperationResolver operationResolver, bool allowedPrerelease = true)
        {
            try
            {
                var operations = operationResolver.ResolveOperations(packageDetails.Package);
                foreach (var operation in operations)
                {
                    switch (operation.Action)
                    {
                        case PackageAction.Install:
                            _packageManager.InstallPackage(operation.Package, true, allowedPrerelease);
                            break;
                        case PackageAction.Uninstall:
                            _packageManager.UninstallPackage(operation.Package, false, false);
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Log(MessageLevel.Error, exception.Message);
            }
        }        
        #endregion
    }
}