﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Common;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Resolver;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Models;

    internal class PackageOperationService : IPackageOperationService
    {
        #region Fields
        private readonly IRepository _localRepository; //todo was IPackageRepository
        private readonly SourceRepository _localSourceRepository;
        private readonly ILogger _logger;
        private readonly INuGetPackageManager _nuGetPackageManager;
        private readonly IPackageOperationContextService _packageOperationContextService;
        //private readonly IRepositoryCacheService _repositoryCacheService;
        private readonly IApiPackageRegistry _apiPackageRegistry;
        private readonly IPackageOperationNotificationService _packageOperationNotificationService;
        private readonly IExtensibleProject _defaultProject;
        #endregion

        #region Constructors
        public PackageOperationService(IPackageOperationContextService packageOperationContextService, ILogger logger, INuGetPackageManager nuGetPackageManager,
            IRepositoryService repositoryService, IApiPackageRegistry apiPackageRegistry, IDefaultExtensibleProjectProvider defaultExtensibleProjectProvider,
            ISourceRepositoryProvider sourceRepositoryProvider, IPackageOperationNotificationService packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => nuGetPackageManager);
            Argument.IsNotNull(() => repositoryService);
            //Argument.IsNotNull(() => repositoryCacheService);
            Argument.IsNotNull(() => apiPackageRegistry);
            Argument.IsNotNull(() => sourceRepositoryProvider);
            Argument.IsNotNull(() => defaultExtensibleProjectProvider);
            Argument.IsNotNull(() => packageOperationNotificationService);

            _packageOperationContextService = packageOperationContextService;
            _logger = logger;
            _nuGetPackageManager = nuGetPackageManager;
            //_repositoryCacheService = repositoryCacheService;
            _apiPackageRegistry = apiPackageRegistry;
            _packageOperationNotificationService = packageOperationNotificationService;
            _defaultProject = defaultExtensibleProjectProvider.GetDefaultProject();
            _localSourceRepository = _defaultProject.AsSourceRepository(sourceRepositoryProvider);
            _localRepository = repositoryService.LocalRepository;

            DependencyVersion = DependencyBehavior.Lowest;  //todo use it into resolver, which replaced old InstallWalker
        }
        #endregion

        #region Properties
        internal DependencyBehavior DependencyVersion { get; set; }
        #endregion

        #region Methods
        public async Task UninstallPackageAsync(IPackageDetails package, CancellationToken token = default)
        {
            Argument.IsNotNull(() => package);

            var uninstalledIdentity = package.GetIdentity();
            var operationPath = _defaultProject.GetInstallPath(uninstalledIdentity);

            try
            {
                //nuPackage should provide identity of installed package, which targeted for uninstall action
                _packageOperationNotificationService.NotifyOperationStarting(operationPath, PackageOperationType.Uninstall, package);
                await _nuGetPackageManager.UninstallPackageForProjectAsync(_defaultProject, package.GetIdentity(), token);
            }
            catch (Exception exception)
            {
                await _logger.LogAsync(LogLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.Exceptions.Add(exception);
            }
            finally
            {
                _packageOperationNotificationService.NotifyOperationFinished(operationPath, PackageOperationType.Uninstall, package);
            }
        }

        public async Task InstallPackageAsync(IPackageDetails package, bool allowedPrerelease = false, CancellationToken token = default)
        {
            Argument.IsNotNull(() => package);

            var installedIdentity = package.GetIdentity();
            var operationPath = _defaultProject.GetInstallPath(installedIdentity);

            try
            {
                ValidatePackage(package);

                //repositories retrieved inside package manager now
                //todo use PackageOperationContextService instead on repositoryContextService

                //here was used a flag 'ignoreDependencies = false' and 'ignoreWalkInfo = false' in old code

                _packageOperationNotificationService.NotifyOperationStarting(operationPath, PackageOperationType.Install, package);
                await _nuGetPackageManager.InstallPackageForProjectAsync(_defaultProject, package.GetIdentity(), token);
            }
            catch (Exception exception)
            {
                await _logger.LogAsync(LogLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.Exceptions.Add(exception);
            }
            finally
            {
                _packageOperationNotificationService.NotifyOperationFinished(operationPath, PackageOperationType.Install, package);
            }
        }


        public async Task UpdatePackagesAsync(IPackageDetails package, bool allowedPrerelease, CancellationToken token = default)
        {
            Argument.IsNotNull(() => package);

            var updateIdentity = package.GetIdentity();
            var installPath = _defaultProject.GetInstallPath(updateIdentity);

            //create current version identity
            var currentVersion = await _nuGetPackageManager.GetVersionInstalledAsync(_defaultProject, updateIdentity.Id, token);
            var currentIdentity = new PackageIdentity(updateIdentity.Id, currentVersion);
            var uninstallPath = _defaultProject.GetInstallPath(currentIdentity);

            try
            {
                ValidatePackage(package);

                //where to get target version?
                //somehow we should get target version from package
                //package should provide 'update' identity

                _packageOperationNotificationService.NotifyOperationStarting(installPath, PackageOperationType.Update, package); //install path is same as update

                //notify about uninstall and install because update in fact is combination of these actions
                //this also allow us provide different InstallPaths on notifications

                _packageOperationNotificationService.NotifyOperationStarting(uninstallPath, PackageOperationType.Uninstall, package);

                _packageOperationNotificationService.NotifyOperationStarting(installPath, PackageOperationType.Install, package); 

                await _nuGetPackageManager.UpdatePackageForProjectAsync(_defaultProject, updateIdentity.Id, updateIdentity.Version, token);

            }
            catch (Exception exception)
            {
                await _logger.LogAsync(LogLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.Exceptions.Add(exception);
            }
            finally
            {
                _packageOperationNotificationService.NotifyOperationFinished(uninstallPath, PackageOperationType.Uninstall, package);
                _packageOperationNotificationService.NotifyOperationFinished(installPath, PackageOperationType.Install, package);
                _packageOperationNotificationService.NotifyOperationFinished(installPath, PackageOperationType.Update, package);
            }
        }

        private void ValidatePackage(IPackageDetails package)
        {
            package.ResetValidationContext();

            _apiPackageRegistry.Validate(package);

            if (package.ValidationContext.GetErrorCount(ValidationTags.Api) > 0)
            {
                throw new ApiValidationException(package.ValidationContext.GetErrors(ValidationTags.Api).First().Message);
            }
        }

        //private PackageWrapper EnsurePackageDependencies(IPackage nuGetPackage)
        //{
        //    List<PackageDependencySet> dependencySets = new List<PackageDependencySet>();
        //    foreach (PackageDependencySet dependencySet in nuGetPackage.DependencySets)
        //    {
        //        dependencySets.Add(new PackageDependencySet(dependencySet.TargetFramework, dependencySet.Dependencies.Where(dependency => !_apiPackageRegistry.IsRegistered(dependency.Id))));
        //    }

        //    return new PackageWrapper(nuGetPackage, dependencySets);
        //}
        #endregion
    }
}
