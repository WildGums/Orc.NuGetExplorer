namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
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

        private readonly IBusyIndicatorService _busyIndicatorService;

        public PackageCommandService(IBusyIndicatorService busyIndicatorService, IRepositoryService repositoryService, IPackageQueryService packageQueryService, IPackageOperationService packageOperationService,
            IPackageOperationContextService packageOperationContextService, IApiPackageRegistry apiPackageRegistry)
        {
            ArgumentNullException.ThrowIfNull(busyIndicatorService);
            ArgumentNullException.ThrowIfNull(repositoryService);
            ArgumentNullException.ThrowIfNull(packageQueryService);
            ArgumentNullException.ThrowIfNull(packageOperationService);
            ArgumentNullException.ThrowIfNull(packageOperationContextService);
            ArgumentNullException.ThrowIfNull(apiPackageRegistry);

            _busyIndicatorService = busyIndicatorService;
            _packageQueryService = packageQueryService;
            _packageOperationService = packageOperationService;
            _packageOperationContextService = packageOperationContextService;
            _apiPackageRegistry = apiPackageRegistry;

            _localRepository = repositoryService.LocalRepository;
        }

        public string GetActionName(PackageOperationType operationType)
        {
            return Enum.GetName(typeof(PackageOperationType), operationType) ?? string.Empty;
        }

        public async Task ExecuteAsync(PackageOperationType operationType, IPackageDetails packageDetails)
        {
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
            using (_busyIndicatorService.PushInScope())
            using (_packageOperationContextService.UseOperationContext(PackageOperationType.Install, packageDetails))
            {
                await _packageOperationService.InstallPackageAsync(packageDetails, token: token);
            }
        }

        public async Task ExecuteInstallAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token)
        {
            using (_busyIndicatorService.PushInScope())
            {
                await _packageOperationService.InstallPackageAsync(packageDetails, token: token);
            }
        }

        public async Task ExecuteUninstallAsync(IPackageDetails packageDetails, CancellationToken token)
        {
            using (_busyIndicatorService.PushInScope())
            using (_packageOperationContextService.UseOperationContext(PackageOperationType.Uninstall, packageDetails))
            {
                await _packageOperationService.UninstallPackageAsync(packageDetails, token: token);
            }
        }

        public async Task ExecuteUpdateAsync(IPackageDetails packageDetails, CancellationToken token)
        {
            using (_busyIndicatorService.PushInScope())
            using (_packageOperationContextService.UseOperationContext(PackageOperationType.Update, packageDetails))
            {
                await _packageOperationService.UpdatePackagesAsync(packageDetails, token: token);
            }
        }

        public async Task ExecuteUpdateAsync(IPackageDetails packageDetails, IDisposable packageOperationContext, CancellationToken token)
        {
            using (_busyIndicatorService.PushInScope())
            {
                await _packageOperationService.UpdatePackagesAsync(packageDetails, token: token);
            }
        }

        public async Task<bool> CanExecuteAsync(PackageOperationType operationType, IPackageDetails package)
        {
            if (package is null)
            {
                Log.Debug("Cannot execute command for null package");
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

        private async Task<IPackageDetails?> GetPackageDetailsFromSelectedVersionAsync(IPackageDetails packageDetails, IRepository repository)
        {
            ArgumentNullException.ThrowIfNull(packageDetails);

            if (!string.IsNullOrWhiteSpace(packageDetails.SelectedVersion) && packageDetails.Version.ToString() != packageDetails.SelectedVersion)
            {
                var details = await _packageQueryService.GetPackageAsync(repository, packageDetails.Id, packageDetails.SelectedVersion);
                return details;
            }

            return packageDetails;
        }

        internal async Task<bool> CanInstallAsync(IPackageDetails package)
        {
            var packageExists = await VerifyLocalPackageExistsAsync(package);

            Log.Debug($"Can install for '{package}': {packageExists}");

            return !packageExists;
        }

        internal async Task<bool> CanUpdateAsync(IPackageDetails package)
        {
            var packageExists = await VerifyLocalPackageExistsAsync(package);

            Log.Debug($"Can update for '{package}': {packageExists}");

            return packageExists;
        }

        internal async Task<bool> VerifyLocalPackageExistsAsync(IPackageDetails package)
        {
            ArgumentNullException.ThrowIfNull(package);

            if (package.IsInstalled is null)
            {
                Log.Debug($"Package '{package}' IsInstalled is null, checking package existence now");

                package.IsInstalled = await _packageQueryService.PackageExistsAsync(_localRepository, package.Id);

                ValidatePackage(package);
            }

            if (package.ValidationContext?.HasErrors ?? false)
            {
                Log.Debug($"Package '{package}' has validation errors, package is not available locally");

                LogValidationErrors(package);

                return false;
            }

            if (!package.IsInstalled.HasValue)
            {
                Log.Debug($"Package '{package}' IsInstalled value is null, package is not available locally");

                return false;
            }

            Log.Debug($"Package '{package}' IsInstalled value is '{package.IsInstalled}'");

            return package.IsInstalled.Value;
        }

        private void LogValidationErrors(IPackageDetails package)
        {
            ArgumentNullException.ThrowIfNull(package);

            if (package.ValidationContext is null)
            {
                return;
            }

            foreach (var error in package.ValidationContext.GetErrors())
            {
                Log.Info($"{package} doesn't satisfy validation rule with error '{error.Message}'");
            }
        }

        private void ValidatePackage(IPackageDetails package)
        {
            ArgumentNullException.ThrowIfNull(package);

            package.ResetValidationContext();

            _apiPackageRegistry.Validate(package);
        }
    }
}
