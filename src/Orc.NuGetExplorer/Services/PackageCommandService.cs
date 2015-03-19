// --------------------------------------------------------------------------------------------------------------------
// <copyright file="packageCommandService.cs" company="Wild Gums">
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

    internal class PackageCommandService : IPackageCommandService
    {
        #region Fields
        private readonly IPackageRepository _localRepository;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPackageOperationService _packageOperationService;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public PackageCommandService(IPleaseWaitService pleaseWaitService, IPackageRepositoryService packageRepositoryService, 
            IPackageQueryService packageQueryService, IPackageOperationService packageOperationService)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => packageOperationService);

            _pleaseWaitService = pleaseWaitService;
            _packageQueryService = packageQueryService;
            _packageOperationService = packageOperationService;

            _localRepository = packageRepositoryService.LocalRepository;
        }
        #endregion

        #region Methods
        public string GetActionName(PackageOperationType operationType)
        {
            return Enum.GetName(typeof (PackageOperationType), operationType);
        }

        public async Task Execute(PackageOperationType operationType, PackageDetails packageDetails, IPackageRepository sourceRepository = null, bool allowedPrerelease = false)
        {
            Argument.IsNotNull(() => packageDetails);

            await Task.Factory.StartNew(() =>
            {
                using (_pleaseWaitService.WaitingScope())
                {
                    switch (operationType)
                    {
                        case PackageOperationType.Uninstall:
                            _packageOperationService.UninstallPackage(packageDetails);
                            break;
                        case PackageOperationType.Install:
                            _packageOperationService.InstallPackage(packageDetails, allowedPrerelease);
                            break;
                        case PackageOperationType.Update:
                            _packageOperationService.UpdatePackages(packageDetails, allowedPrerelease);
                            break;
                    }
                }
            });

            packageDetails.IsActionExecuted = null;
        }

        public bool CanExecute(PackageOperationType operationType, PackageDetails package)
        {
            if (package == null)
            {
                return false;
            }

            if (package.IsActionExecuted == null)
            {
                switch (operationType)
                {
                    case PackageOperationType.Install:
                        package.IsActionExecuted = !CanInstall(package);
                        break;

                    case PackageOperationType.Update:
                        package.IsActionExecuted = !CanUpdate(package);
                        break;

                    case PackageOperationType.Uninstall:
                        package.IsActionExecuted = !CanUninstall(package);
                        break;

                    default:
                        package.IsActionExecuted = null;
                        break;
                }
            }

            return !(package.IsActionExecuted ?? true);
        }

        public bool IsRefreshReqired(PackageOperationType operationType)
        {
            switch (operationType)
            {
                case PackageOperationType.Uninstall:
                    return true;
                case PackageOperationType.Install:
                    return false;
                case PackageOperationType.Update:
                    return true;
            }

            return false;
        }

        public string GetPluralActionName(PackageOperationType operationType)
        {
            return string.Format("{0} all", Enum.GetName(typeof (PackageOperationType), operationType));
        }

        private bool CanInstall(PackageDetails package)
        {
            Argument.IsNotNull(() => package);

            var count = _packageQueryService.CountPackages(_localRepository, package.Id);

            return count == 0;
        }

        private bool CanUpdate(PackageDetails package)
        {
            Argument.IsNotNull(() => package);

            var count = _packageQueryService.CountPackages(_localRepository, package);

            return count == 0;
        }

        private bool CanUninstall(PackageDetails package)
        {
            return true;
        }                        
        #endregion
    }
}