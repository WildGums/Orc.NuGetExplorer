// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCommandService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    using Catel;
    using Catel.Services;

    internal class PackageCommandService : IPackageCommandService
    {
        #region Fields
        private readonly IApiPackageRegistry _apiPackageRegistry;

        private readonly IRepository _localRepository;

        private readonly IPackageOperationContextService _packageOperationContextService;

        private readonly IPackageOperationService _packageOperationService;

        private readonly IPackageQueryService _packageQueryService;

        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public PackageCommandService(IPleaseWaitService pleaseWaitService, IRepositoryService repositoryService, IPackageQueryService packageQueryService, IPackageOperationService packageOperationService, IPackageOperationContextService packageOperationContextService, IApiPackageRegistry apiPackageRegistry)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => packageOperationService);
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => apiPackageRegistry);

            _pleaseWaitService = pleaseWaitService;
            _packageQueryService = packageQueryService;
            _packageOperationService = packageOperationService;
            _packageOperationContextService = packageOperationContextService;
            _apiPackageRegistry = apiPackageRegistry;

            _localRepository = repositoryService.LocalRepository;
        }
        #endregion

        #region Methods
        public string GetActionName(PackageOperationType operationType)
        {
            return Enum.GetName(typeof(PackageOperationType), operationType);
        }

        public void Execute(PackageOperationType operationType, IPackageDetails packageDetails, IRepository sourceRepository = null, bool allowedPrerelease = false)
        {
            Argument.IsNotNull(() => packageDetails);

            var selectedPackage = GetPackageDetailsFromSelectedVersion(packageDetails, sourceRepository ?? _localRepository) ?? packageDetails;

            using (_pleaseWaitService.WaitingScope())
            {
                using (_packageOperationContextService.UseOperationContext(operationType, selectedPackage))
                {
                    _packageOperationContextService.CurrentContext.Repository = sourceRepository;
                    switch (operationType)
                    {
                        case PackageOperationType.Uninstall:
                            _packageOperationService.UninstallPackage(selectedPackage);
                            break;

                        case PackageOperationType.Install:
                            _packageOperationService.InstallPackage(selectedPackage, allowedPrerelease);
                            break;

                        case PackageOperationType.Update:
                            _packageOperationService.UpdatePackages(selectedPackage, allowedPrerelease);
                            break;
                    }
                }
            }

            packageDetails.IsInstalled = null;
        }

        public bool CanExecute(PackageOperationType operationType, IPackageDetails package)
        {
            if (package == null)
            {
                return false;
            }

            var selectedPackage = GetPackageDetailsFromSelectedVersion(package, _localRepository) ?? package;

            switch (operationType)
            {
                case PackageOperationType.Install:
                    return CanInstall(selectedPackage);

                case PackageOperationType.Update:
                    return CanUpdate(selectedPackage);

                case PackageOperationType.Uninstall:
                    return true;
            }

            return false;
        }

        public bool IsRefreshRequired(PackageOperationType operationType)
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
            return $"{Enum.GetName(typeof(PackageOperationType), operationType)} all";
        }

        private IPackageDetails GetPackageDetailsFromSelectedVersion(IPackageDetails packageDetails, IRepository repository)
        {
            if (!string.IsNullOrWhiteSpace(packageDetails.SelectedVersion) && packageDetails.Version.ToString() != packageDetails.SelectedVersion)
            {
                packageDetails = _packageQueryService.GetPackage(repository, packageDetails.Id, packageDetails.SelectedVersion);
            }

            return packageDetails;
        }

        private bool CanInstall(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            if (package.IsInstalled == null)
            {
                var count = _packageQueryService.CountPackages(_localRepository, package.Id);
                package.IsInstalled = count != 0;
                ValidatePackage(package);
            }

            return package.IsInstalled != null && !package.IsInstalled.Value && package.ValidationContext.GetErrorCount(ValidationTags.Api) == 0;
        }

        private void ValidatePackage(IPackageDetails package)
        {
            package.ResetValidationContext();
            _apiPackageRegistry.Validate(package);
        }

        private bool CanUpdate(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            if (package.IsInstalled == null)
            {
                var count = _packageQueryService.CountPackages(_localRepository, package);
                package.IsInstalled = count != 0;

                ValidatePackage(package);
            }

            return !package.IsInstalled.Value && package.ValidationContext.GetErrorCount(ValidationTags.Api) == 0;
        }
        #endregion
    }
}
