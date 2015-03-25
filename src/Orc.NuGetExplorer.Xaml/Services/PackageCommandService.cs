// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCommandService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;

    internal class PackageCommandService : IPackageCommandService
    {
        #region Fields
        private readonly IRepository _localRepository;
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IPackageOperationService _packageOperationService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public PackageCommandService(IPleaseWaitService pleaseWaitService, IPackageRepositoryService packageRepositoryService,
            IPackageQueryService packageQueryService, IPackageOperationService packageOperationService, IPackageOperationContextService packageOperationContextService)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => packageOperationService);
            Argument.IsNotNull(() => packageOperationContextService);

            _pleaseWaitService = pleaseWaitService;
            _packageQueryService = packageQueryService;
            _packageOperationService = packageOperationService;
            _packageOperationContextService = packageOperationContextService;

            _localRepository = packageRepositoryService.LocalRepository;
        }
        #endregion

        #region Methods
        public string GetActionName(PackageOperationType operationType)
        {
            return Enum.GetName(typeof (PackageOperationType), operationType);
        }

        public async Task Execute(PackageOperationType operationType, IPackageDetails packageDetails, IRepository sourceRepository = null, bool allowedPrerelease = false)
        {
            Argument.IsNotNull(() => packageDetails);

            await Task.Factory.StartNew(() =>
            {
               // using (_pleaseWaitService.WaitingScope())
                {
                    using (_packageOperationContextService.UseOperationContext(operationType, packageDetails))
                    {
                        _packageOperationContextService.CurrentContext.Repository = sourceRepository;
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
                }
            });

            packageDetails.IsInstalled = null;
        }

        public bool CanExecute(PackageOperationType operationType, IPackageDetails package)
        {
            if (package == null)
            {
                return false;
            }

            switch (operationType)
            {
                case PackageOperationType.Install:
                    return CanInstall(package);

                case PackageOperationType.Update:
                    return CanUpdate(package);

                case PackageOperationType.Uninstall:
                    return CanUninstall(package);
                    ;
            }

            return false;
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

        private bool CanInstall(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            if (package.IsInstalled == null)
            {
                var count = _packageQueryService.CountPackages(_localRepository, package.Id);
                package.IsInstalled = count != 0;
            }

            return !package.IsInstalled.Value;
        }

        private bool CanUpdate(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            if (package.IsInstalled == null)
            {
                var count = _packageQueryService.CountPackages(_localRepository, package);
                package.IsInstalled = count != 0;
            }

            return !package.IsInstalled.Value;
        }

        private bool CanUninstall(IPackageDetails package)
        {
            return true;
        }
        #endregion
    }
}