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
    using Catel.Logging;
    using Catel.Services;

    internal class PackageCommandService : IPackageCommandService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IApiPackageRegistry _apiPackageRegistry;

        private readonly IRepository _localRepository;

        private readonly IPackageOperationContextService _packageOperationContextService;

        private readonly IPackageOperationService _packageOperationService;

        private readonly IPackageQueryService _packageQueryService;

        private readonly IPleaseWaitService _pleaseWaitService;

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

        public string GetActionName(PackageOperationType operationType)
        {
            return Enum.GetName(typeof(PackageOperationType), operationType);
        }

        [ObsoleteEx(TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0", ReplacementTypeOrMember = "ExecuteAsync")]
        public async Task ExecuteAsync(PackageOperationType operationType, IPackageDetails packageDetails, IRepository sourceRepository = null, bool allowedPrerelease = false)
        {
            await ExecuteAsync(operationType, packageDetails);
        }

        public async Task ExecuteAsync(PackageOperationType operationType, IPackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            switch (operationType)
            {
                case PackageOperationType.Uninstall:
                    await ExecuteUninstallAsync(packageDetails, default);
                    break;

                case PackageOperationType.Install:
                    await ExecuteInstallAsync(packageDetails, default);
                    break;

                case PackageOperationType.Update:
                    await ExecuteUpdateAsync(packageDetails, default);
                    break;
            }
        }

        public async Task ExecuteInstallAsync(IPackageDetails packageDetails, CancellationToken token)
        {
            using (_pleaseWaitService.WaitingScope())
            using (_packageOperationContextService.UseOperationContext(PackageOperationType.Install, packageDetails))
            {
                await _packageOperationService.InstallPackageAsync(packageDetails, token: token);
            }
        }

        public async Task ExecuteInstallAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token)
        {
            using (_pleaseWaitService.WaitingScope())
            {
                await _packageOperationService.InstallPackageAsync(packageDetails, token: token);
            }
        }

        public async Task ExecuteUninstallAsync(IPackageDetails packageDetails, CancellationToken token)
        {
            using (_pleaseWaitService.WaitingScope())
            using (_packageOperationContextService.UseOperationContext(PackageOperationType.Uninstall, packageDetails))
            {
                await _packageOperationService.UninstallPackageAsync(packageDetails, token: token);
            }
        }

        public async Task ExecuteUpdateAsync(IPackageDetails packageDetails, CancellationToken token)
        {
            using (_pleaseWaitService.WaitingScope())
            using (_packageOperationContextService.UseOperationContext(PackageOperationType.Update, packageDetails))
            {
                await _packageOperationService.UpdatePackagesAsync(packageDetails, token: token);
            }
        }

        public async Task ExecuteUpdateAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token)
        {
            using (_pleaseWaitService.WaitingScope())
            {
                await _packageOperationService.UpdatePackagesAsync(packageDetails, token: token);
            }
        }

        public async Task<bool> CanExecuteAsync(PackageOperationType operationType, IPackageDetails package)
        {
            if (package is null)
            {
                return false;
            }

            var selectedPackage = await GetPackageDetailsFromSelectedVersionAsync(package, _localRepository) ?? package;

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

        private async Task<IPackageDetails> GetPackageDetailsFromSelectedVersionAsync(IPackageDetails packageDetails, IRepository repository)
        {
            if (!string.IsNullOrWhiteSpace(packageDetails.SelectedVersion) && packageDetails.Version.ToString() != packageDetails.SelectedVersion)
            {
                packageDetails = await _packageQueryService.GetPackageAsync(repository, packageDetails.Id, packageDetails.SelectedVersion);
            }

            return packageDetails;
        }

        private Task<bool> CanInstallAsync(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            return VerifyLocalPackageExistsAsync(package);
        }

        private Task<bool> CanUpdateAsync(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            return VerifyLocalPackageExistsAsync(package);
        }

        private async Task<bool> VerifyLocalPackageExistsAsync(IPackageDetails package)
        {
            if (package.IsInstalled is null)
            {
                package.IsInstalled = await _packageQueryService.PackageExistsAsync(_localRepository, package.Id);
                ValidatePackage(package);
            }

            if (package.ValidationContext.HasErrors)
            {
                LogValidationErrors(package);

                return false;
            }

            if (!package.IsInstalled.HasValue)
            {
                return false;
            }

            return !package.IsInstalled.Value;
        }

        private void LogValidationErrors(IPackageDetails package)
        {
            foreach (var error in package.ValidationContext.GetErrors())
            {
                Log.Info($"{package} doesn't satisfy validation rule with error '{error.Message}'");
            }
        }

        private void ValidatePackage(IPackageDetails package)
        {
            package.ResetValidationContext();
            _apiPackageRegistry.Validate(package);
        }
    }
}
