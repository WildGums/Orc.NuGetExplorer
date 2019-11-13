// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCommandService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
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
        public PackageCommandService(IPleaseWaitService pleaseWaitService, IRepositoryService repositoryService, IPackageQueryService packageQueryService, IPackageOperationService packageOperationService, 
            IPackageOperationContextService packageOperationContextService, IApiPackageRegistry apiPackageRegistry)
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

        public async Task ExecuteAsync(PackageOperationType operationType, IPackageDetails packageDetails, IRepository sourceRepository = null, bool allowedPrerelease = false)
        {
            Argument.IsNotNull(() => packageDetails);

            var selectedPackage = await GetPackageDetailsFromSelectedVersion(packageDetails, sourceRepository ?? _localRepository) ?? packageDetails;

            using (_pleaseWaitService.WaitingScope())
            {
                using (_packageOperationContextService.UseOperationContext(operationType, selectedPackage))
                {
                    _packageOperationContextService.CurrentContext.Repository = sourceRepository;
                    switch (operationType)
                    {
                        case PackageOperationType.Uninstall:
                            await _packageOperationService.UninstallPackageAsync(selectedPackage);
                            break;

                        case PackageOperationType.Install:
                            await _packageOperationService.InstallPackageAsync(selectedPackage, allowedPrerelease);
                            break;

                        case PackageOperationType.Update:
                            await _packageOperationService.UpdatePackagesAsync(selectedPackage, allowedPrerelease);
                            break;
                    }
                }
            }

            packageDetails.IsInstalled = null;
        }

        public async Task<bool> CanExecuteAsync(PackageOperationType operationType, IPackageDetails package)
        {
            if (package == null)
            {
                return false;
            }

            var selectedPackage = await GetPackageDetailsFromSelectedVersion(package, _localRepository) ?? package;

            switch (operationType)
            {
                case PackageOperationType.Install:
                    return await CanInstallAsync(selectedPackage);

                case PackageOperationType.Update:
                    return await CanUpdateAsync(selectedPackage);

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

        private async Task<IPackageDetails> GetPackageDetailsFromSelectedVersion(IPackageDetails packageDetails, IRepository repository)
        {
            if (!string.IsNullOrWhiteSpace(packageDetails.SelectedVersion) && packageDetails.Version.ToString() != packageDetails.SelectedVersion)
            {
                packageDetails = await _packageQueryService.GetPackage(repository, packageDetails.Id, packageDetails.SelectedVersion);
            }

            return packageDetails;
        }

        private async Task<bool> CanInstallAsync(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            if (package.IsInstalled == null)
            {
                package.IsInstalled = await _packageQueryService.PackageExists(_localRepository, package.Id);
                ValidatePackage(package);
            }

            return package.IsInstalled != null && !package.IsInstalled.Value && package.ValidationContext.GetErrorCount(ValidationTags.Api) == 0;
        }

        private void ValidatePackage(IPackageDetails package)
        {
            package.ResetValidationContext();
            _apiPackageRegistry.Validate(package);
        }

        private async Task<bool> CanUpdateAsync(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            if (package.IsInstalled == null)
            {
                package.IsInstalled = await _packageQueryService.PackageExists(_localRepository, package);

                ValidatePackage(package);
            }

            return !package.IsInstalled.Value && package.ValidationContext.GetErrorCount(ValidationTags.Api) == 0;
        }
        #endregion
    }
}
