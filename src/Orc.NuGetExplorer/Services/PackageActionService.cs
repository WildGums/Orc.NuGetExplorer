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
        private readonly IPackageQueryService _packageQueryService;
        private readonly INuGetPackageManager _packageManager;
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly IPackageRepository _localRepository;
        #endregion

        #region Constructors
        public PackageActionService(IPleaseWaitService pleaseWaitService, INuGetPackageManager packageManager,
            IPackageRepositoryService packageRepositoryService, ILogger logger, IPackageQueryService packageQueryService)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageManager);
            Argument.IsNotNull(() => packageRepositoryService);
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => packageQueryService);

            _pleaseWaitService = pleaseWaitService;
            _packageManager = packageManager;
            _logger = logger;
            _packageQueryService = packageQueryService;

            _localRepository = packageRepositoryService.LocalRepository;

            DependencyVersion = DependencyVersion.Lowest;
        }
        #endregion

        #region Properties
        public DependencyVersion DependencyVersion { get; set; }
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
                            UpdatePackages(packageDetails, allowedPrerelease);
                            break;
                    }
                }
            });
        }

        public bool CanExecute(RepositoryCategoryType repositoryCategory, PackageDetails packageDetails)
        {
            if (packageDetails == null)
            {
                return false;
            }

            if (repositoryCategory == RepositoryCategoryType.Online)
            {
                if (packageDetails.IsInstalled == null)
                {
                    var count = _packageQueryService.CountPackages(_localRepository, packageDetails.Id);
                    packageDetails.IsInstalled = count != 0;
                    return count == 0;
                }

                return !packageDetails.IsInstalled.Value;
            }
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

            using (_packageManager.OperationsBatchContext(packageDetails, PackageOperationType.Uninstall))
            {
                var dependentsResolver = new DependentsWalker(remoteRepository, null);

                var walker = new UninstallWalker(_localRepository, dependentsResolver, null,
                    _logger, true, false);

                try
                {
                    var operations = walker.ResolveOperations(packageDetails.Package);// checking uninstall ability

                    _packageManager.UninstallPackage(packageDetails.Package, false, true);
                }
                catch (Exception exception)
                {
                    _logger.Log(MessageLevel.Error, exception.Message);
                }
            }            
        }

        private void InstallPackage(IPackageRepository remoteRepository, PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => remoteRepository);
            Argument.IsNotNull(() => packageDetails);

            using (_packageManager.OperationsBatchContext(packageDetails, PackageOperationType.Install))
            {
                var walker = new InstallWalker(_localRepository, remoteRepository, null, _logger, false, allowedPrerelease,
                DependencyVersion);

                try
                {
                    var operations = walker.ResolveOperations(packageDetails.Package);// checking install ability

                    _packageManager.InstallPackage(packageDetails.Package, false, allowedPrerelease, false);
                }
                catch (Exception exception)
                {
                    _logger.Log(MessageLevel.Error, exception.Message);
                }
            }            
        }

        private void UpdatePackages(PackageDetails packageDetails, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => packageDetails);

            using (_packageManager.OperationsBatchContext(packageDetails, PackageOperationType.Update))
            {
                try
                {
                    _packageManager.UpdatePackage(packageDetails.Package, true, allowedPrerelease);
                }
                catch (Exception exception)
                {
                    _logger.Log(MessageLevel.Error, exception.Message);
                }
            }            
        }
        #endregion
    }
}