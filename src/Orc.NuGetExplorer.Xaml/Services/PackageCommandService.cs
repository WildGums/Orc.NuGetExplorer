// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCommandService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using NuGet.Common;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Packaging;
    using Orc.NuGetExplorer.Providers;

    internal class PackageCommandService : IPackageCommandService
    {
        #region Fields
        private readonly IApiPackageRegistry _apiPackageRegistry;
        private readonly IPackageMetadataProvider _packageMetadataProvider;
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IPackageOperationService _packageOperationService;
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public PackageCommandService(IPleaseWaitService pleaseWaitService, IRepositoryService repositoryService, IPackageOperationService packageOperationService,
            IPackageOperationContextService packageOperationContextService, IApiPackageRegistry apiPackageRegistry, IPackageMetadataProvider packageMetadataProvider, ILogger logger)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageOperationService);
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => apiPackageRegistry);
            Argument.IsNotNull(() => packageMetadataProvider);
            Argument.IsNotNull(() => logger);

            _pleaseWaitService = pleaseWaitService;
            _packageOperationService = packageOperationService;
            _packageOperationContextService = packageOperationContextService;
            _apiPackageRegistry = apiPackageRegistry;
            _packageMetadataProvider = packageMetadataProvider;
            _logger = logger;
        }
        #endregion

        #region Methods

        // TODO: Add context with parameters - source/prerelease
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

            var selectedPackage = await GetPackageDetailsFromSelectedVersionAsync(package) ?? package;

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

        private async Task<IPackageDetails> GetPackageDetailsFromSelectedVersionAsync(IPackageDetails packageDetails)
        {
            if (!string.IsNullOrWhiteSpace(packageDetails.SelectedVersion) && packageDetails.Version.ToString() != packageDetails.SelectedVersion)
            {
                packageDetails = await GetPackageAsync(packageDetails.Id);
            }

            return packageDetails;
        }

        private async Task<bool> CanInstallAsync(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            if (package.IsInstalled is null)
            {
                package.IsInstalled = await PackageExistsLocallyAsync(package.Id);
                ValidatePackage(package);
            }

            return package.IsInstalled is not null && !package.IsInstalled.Value && package.ValidationContext.GetErrorCount(ValidationTags.Api) == 0;
        }

        private async Task<bool> CanUpdateAsync(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);

            package.IsInstalled ??= await PackageExistsLocallyAsync(package.Id);
            ValidatePackage(package);

            return !package.IsInstalled.Value && package.ValidationContext.GetErrorCount(ValidationTags.Api) == 0;
        }

        private void ValidatePackage(IPackageDetails package)
        {
            package.ResetValidationContext();
            _apiPackageRegistry.Validate(package);
        }

        private async Task<bool> PackageExistsLocallyAsync(string packageId)
        {
            var packageVersionedMetadata = await _packageMetadataProvider.GetLowestLocalPackageMetadataAsync(packageId, true, CancellationToken.None);
            return packageVersionedMetadata is not null;
        }

        public async Task<IPackageDetails> GetPackageAsync(string packageId)
        {
            return await BuildMultiVersionPackageSearchMetadataAsync(packageId, true);
        }

        // TODO: take should use configured value
        public async Task<IEnumerable<IPackageDetails>> GetPackagesAsync(SourceRepository repository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10)
        {
            var searchResource = await repository.GetResourceAsync<PackageSearchResource>();

            var searchFilters = new SearchFilter(allowPrereleaseVersions);

            var packages = await searchResource.SearchAsync(filter ?? string.Empty, searchFilters, skip, take, _logger, CancellationToken.None);

            //provide information about available versions
            var packageDetails = packages.Select(async package => await BuildMultiVersionPackageSearchMetadataAsync(package, allowPrereleaseVersions))
                .Select(x => x.Result)
                .Where(result => result is not null);

            return packageDetails;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> GetVersionsOfPackageAsync(IPackageDetails package, bool allowPrereleaseVersions, int skip)
        {
            if (skip < 0)
            {
                return Enumerable.Empty<IPackageSearchMetadata>();
            }

            // TODO: use specific source repository here;
            var getMetadataResult = await _packageMetadataProvider.GetPackageMetadataListAsync(package.Id, allowPrereleaseVersions, false, CancellationToken.None);

            var versions = getMetadataResult.Skip(skip).ToList();

            return versions;
        }

        private async Task<IPackageDetails> BuildMultiVersionPackageSearchMetadataAsync(IPackageSearchMetadata packageSearchMetadata, bool includePrerelease)
        {
            return await BuildMultiVersionPackageSearchMetadataAsync(packageSearchMetadata.Identity.Id, includePrerelease);
        }

        private async Task<IPackageDetails> BuildMultiVersionPackageSearchMetadataAsync(string packageId, bool includePrerelease)
        {
            // TODO: use specific source repository here;
            var versionsMetadata = await _packageMetadataProvider.GetPackageMetadataListAsync(packageId, includePrerelease, false, CancellationToken.None);

            var details = MultiVersionPackageSearchMetadataBuilder.FromMetadatas(versionsMetadata).Build() as IPackageDetails;

            return details;
        }

        #endregion
    }
}
