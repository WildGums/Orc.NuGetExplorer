﻿namespace Orc.NuGetExplorer;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel.Data;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Messaging;
using Orc.NuGetExplorer.Packaging;

internal sealed class PackageOperationService : IPackageOperationService
{
    private readonly ILogger _logger;
    private readonly INuGetPackageManager _nuGetPackageManager;
    private readonly IPackageOperationContextService _packageOperationContextService;
    private readonly IApiPackageRegistry _apiPackageRegistry;
    private readonly IPackageOperationNotificationService _packageOperationNotificationService;
    private readonly IExtensibleProject _defaultProject;

    public PackageOperationService(IPackageOperationContextService packageOperationContextService, ILogger logger, INuGetPackageManager nuGetPackageManager,
        IRepositoryService repositoryService, IApiPackageRegistry apiPackageRegistry, IDefaultExtensibleProjectProvider defaultExtensibleProjectProvider,
        ISourceRepositoryProvider sourceRepositoryProvider, IPackageOperationNotificationService packageOperationNotificationService)
    {
        ArgumentNullException.ThrowIfNull(packageOperationContextService);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(nuGetPackageManager);
        ArgumentNullException.ThrowIfNull(repositoryService);
        ArgumentNullException.ThrowIfNull(apiPackageRegistry);
        ArgumentNullException.ThrowIfNull(defaultExtensibleProjectProvider);
        ArgumentNullException.ThrowIfNull(sourceRepositoryProvider);
        ArgumentNullException.ThrowIfNull(packageOperationNotificationService);

        _packageOperationContextService = packageOperationContextService;
        _logger = logger;
        _nuGetPackageManager = nuGetPackageManager;
        _apiPackageRegistry = apiPackageRegistry;
        _packageOperationNotificationService = packageOperationNotificationService;
        _defaultProject = defaultExtensibleProjectProvider.GetDefaultProject();

        // Note: this setting should be global, probably set by Resolver (which replaced the old one InstallWalker);
        DependencyVersion = DependencyBehavior.Lowest;
    }

    internal DependencyBehavior DependencyVersion { get; set; }

    public async Task UninstallPackageAsync(IPackageDetails package,
        Func<PackageIdentity, bool>? packagePredicate = null, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(package);

        var uninstalledIdentity = package.GetIdentity();
        var uninstallPath = _defaultProject.GetInstallPath(uninstalledIdentity);

        try
        {
            //nuPackage should provide identity of installed package, which targeted for uninstall action
            _packageOperationNotificationService.NotifyOperationStarting(uninstallPath, PackageOperationType.Uninstall, package);
            await _nuGetPackageManager.UninstallPackageForProjectAsync(_defaultProject, package.GetIdentity(), packagePredicate, token);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(LogLevel.Error, ex.Message);
            _packageOperationContextService.CurrentContext?.Exceptions?.Add(ex);
        }
        finally
        {
            FinishOperation(PackageOperationType.Uninstall, uninstallPath, package);
        }
    }

    public async Task InstallPackageAsync(IPackageDetails package, bool allowedPrerelease = false,
        Func<PackageIdentity, bool>? packagePredicate = null, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(package);

        var installedIdentity = package.GetIdentity();
        var operationPath = _defaultProject.GetInstallPath(installedIdentity);

        try
        {
            ValidatePackage(package);

            //repositories retrieved inside package manager now
            //todo use PackageOperationContextService instead on repositoryContextService

            //here was used a flag 'ignoreDependencies = false' and 'ignoreWalkInfo = false' in old code

            _packageOperationNotificationService.NotifyOperationStarting(operationPath, PackageOperationType.Install, package);
            await _nuGetPackageManager.InstallPackageForProjectAsync(_defaultProject, package.GetIdentity(), packagePredicate, token);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(LogLevel.Error, ex.Message);
            _packageOperationContextService.CurrentContext?.Exceptions?.Add(ex);
        }
        finally
        {
            FinishOperation(PackageOperationType.Install, operationPath, package);
        }
    }

    public async Task UpdatePackagesAsync(IPackageDetails package, bool allowedPrerelease = false,
        Func<PackageIdentity, bool>? packagePredicate = null, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(package);

        var updateIdentity = package.GetIdentity();
        var installPath = _defaultProject.GetInstallPath(updateIdentity);

        // create current version identity
        var currentVersion = await _nuGetPackageManager.GetVersionInstalledAsync(_defaultProject, updateIdentity.Id, token);
        var currentIdentity = new PackageIdentity(updateIdentity.Id, currentVersion);
        if (currentIdentity.Version is null)
        {
            // Can be because of mismatch between packages.config and package files

            _logger.LogWarning($"Could not find existing local files for installed '{package.Id}'. Continue update to version '{updateIdentity.Version}'");

            ValidatePackage(package);
            _packageOperationNotificationService.NotifyOperationStarting(installPath, PackageOperationType.Install, package);

            await _nuGetPackageManager.UpdatePackageForProjectAsync(_defaultProject, updateIdentity.Id, updateIdentity.Version, packagePredicate, token);

            return;
        }
        var uninstallPath = _defaultProject.GetInstallPath(currentIdentity);

        try
        {
            ValidatePackage(package);

            _packageOperationNotificationService.NotifyOperationStarting(installPath, PackageOperationType.Update, package); // install path is same as update

            // notify about uninstall and install because update in fact is combination of these actions
            // this also allow us provide different InstallPaths on notifications

            _packageOperationNotificationService.NotifyOperationStarting(uninstallPath, PackageOperationType.Uninstall, package);

            _packageOperationNotificationService.NotifyOperationStarting(installPath, PackageOperationType.Install, package);

            await _nuGetPackageManager.UpdatePackageForProjectAsync(_defaultProject, updateIdentity.Id, updateIdentity.Version, packagePredicate, token);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(LogLevel.Error, ex.Message);
            _packageOperationContextService.CurrentContext?.Exceptions?.Add(ex);
        }
        finally
        {
            FinishOperation(PackageOperationType.Uninstall, uninstallPath, package);
            FinishOperation(PackageOperationType.Install, installPath, package);
            FinishOperation(PackageOperationType.Update, installPath, package); // The install path the same for update;
        }
    }

    private void ValidatePackage(IPackageDetails package)
    {
        ArgumentNullException.ThrowIfNull(package);

        package.ResetValidationContext();

        _apiPackageRegistry.Validate(package);

        package.ValidationContext ??= new ValidationContext();
        if (package.ValidationContext.GetErrorCount(ValidationTags.Api) > 0)
        {
            throw new ApiValidationException(package.ValidationContext.GetErrors(ValidationTags.Api).First().Message);
        }
    }

    private void FinishOperation(PackageOperationType type, string operationPath, IPackageDetails package)
    {
        PackagingDeletemeMessage.SendWith(new PackageOperationInfo(operationPath, type, package));
        _packageOperationNotificationService.NotifyOperationFinished(operationPath, type, package);
    }
}
