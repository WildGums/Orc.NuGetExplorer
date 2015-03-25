// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using NuGet;

    internal class PackageOperationService : IPackageOperationService
    {
        #region Fields
        private readonly IPackageRepository _localRepository;
        private readonly ILogger _logger;
        private readonly IPackageManager _packageManager;
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IRepositoryCacheService _repositoryCacheService;
        #endregion

        #region Constructors
        public PackageOperationService(IPackageOperationContextService packageOperationContextService, ILogger logger, IPackageManager packageManager,
            IPackageRepositoryService packageRepositoryService, IRepositoryCacheService repositoryCacheService)
        {
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => packageManager);
            Argument.IsNotNull(() => packageRepositoryService);
            Argument.IsNotNull(() => repositoryCacheService);

            _packageOperationContextService = packageOperationContextService;
            _logger = logger;
            _packageManager = packageManager;
            _repositoryCacheService = repositoryCacheService;

            _localRepository = repositoryCacheService.GetNuGetRepository(packageRepositoryService.LocalRepository);

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
                var nuGetPackage = ((PackageDetails) package).Package;
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
                var nuGetPackage = ((PackageDetails) package).Package;
                _packageManager.UpdatePackage(nuGetPackage, true, allowedPrerelease);
            }
            catch (Exception exception)
            {
                _logger.Log(MessageLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.CatchedExceptions.Add(exception);
            }
        }
        #endregion
    }
}