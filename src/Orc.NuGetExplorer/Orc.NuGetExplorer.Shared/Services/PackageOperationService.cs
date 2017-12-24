// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using NuGet;
    using Orc.NuGetExplorer.Services;

    internal class PackageOperationService : IPackageOperationService
    {
        #region Fields
        private readonly IPackageRepository _localRepository;
        private readonly ILogger _logger;
        private readonly IPackageManager _packageManager;
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IRepositoryCacheService _repositoryCacheService;
        private readonly IApiPackageRegistry _apiPackageRegistry;
        #endregion

        #region Constructors
        public PackageOperationService(IPackageOperationContextService packageOperationContextService, ILogger logger, IPackageManager packageManager,
            IRepositoryService repositoryService, IRepositoryCacheService repositoryCacheService, IApiPackageRegistry apiPackageRegistry)
        {
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => packageManager);
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => repositoryCacheService);
            Argument.IsNotNull(() => apiPackageRegistry);

            _packageOperationContextService = packageOperationContextService;
            _logger = logger;
            _packageManager = packageManager;
            _repositoryCacheService = repositoryCacheService;
            _apiPackageRegistry = apiPackageRegistry;

            _localRepository = repositoryCacheService.GetNuGetRepository(repositoryService.LocalRepository);

            DependencyVersion = DependencyVersion.Lowest;
        }
        #endregion

        #region Properties
        internal DependencyVersion DependencyVersion { get; set; }
        #endregion

        #region Methods
        public void UninstallPackage(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);
            Argument.IsOfType(() => package, typeof (PackageDetails));

            var dependentsResolver = new DependentsWalker(_localRepository, null);

            var walker = new UninstallWalker(_localRepository, dependentsResolver, null,
                _logger, true, false);

            try
            {
                var nuGetPackage = ((PackageDetails) package).Package;
                var operations = walker.ResolveOperations(nuGetPackage);
                _packageManager.UninstallPackage(nuGetPackage, false, true);
            }
            catch (Exception exception)
            {
                _logger.Log(MessageLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.CatchedExceptions.Add(exception);
            }
        }

        public void InstallPackage(IPackageDetails package, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => package);
            Argument.IsOfType(() => package, typeof (PackageDetails));

            var repository = _packageOperationContextService.CurrentContext.Repository;
            var sourceRepository = _repositoryCacheService.GetNuGetRepository(repository);

            var walker = new InstallWalker(_localRepository, sourceRepository, null, _logger, false, allowedPrerelease, DependencyVersion);

            try
            {
                _apiPackageRegistry.Validate(package);
                if (package.ApiValidations.Count > 0)
                {
                    throw new ApiValidationException(package.ApiValidations.First());
                }

                var nuGetPackage = EnsurePackageDependencies(((PackageDetails) package).Package);
                var operations = walker.ResolveOperations(nuGetPackage);
                _packageManager.InstallPackage(nuGetPackage, false, allowedPrerelease, false);
            }
            catch (Exception exception)
            {
                _logger.Log(MessageLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.CatchedExceptions.Add(exception);
            }
        }

        public void UpdatePackages(IPackageDetails package, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => package);
            Argument.IsOfType(() => package, typeof (PackageDetails));

            try
            {
                _apiPackageRegistry.Validate(package);
                if (package.ApiValidations.Count > 0)
                {
                    throw new ApiValidationException(package.ApiValidations.First());
                }

                var nuGetPackage = EnsurePackageDependencies(((PackageDetails) package).Package);
                _packageManager.UpdatePackage(nuGetPackage, true, allowedPrerelease);
            }
            catch (Exception exception)
            {
                _logger.Log(MessageLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.CatchedExceptions.Add(exception);
            }
        }

        private PackageWrapper EnsurePackageDependencies(IPackage nuGetPackage)
        {
            List<PackageDependencySet> dependencySets = new List<PackageDependencySet>();
            foreach (PackageDependencySet dependencySet in nuGetPackage.DependencySets)
            {
                dependencySets.Add(new PackageDependencySet(dependencySet.TargetFramework, dependencySet.Dependencies.Where(dependency => !_apiPackageRegistry.IsRegistered(dependency.Id))));
            }

            return new PackageWrapper(nuGetPackage, dependencySets);
        }
        #endregion
    }
}