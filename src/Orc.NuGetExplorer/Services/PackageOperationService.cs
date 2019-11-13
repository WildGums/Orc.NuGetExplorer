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
    using System.Threading;
    using Catel;
    using Catel.Threading;
    using NuGet.Common;
    using NuGet.Protocol.Core.Types;
    using NuGet.Resolver;
    using NuGet.Versioning;
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
        private readonly IExtensibleProject _defaultProject;
        #endregion

        #region Constructors
        public PackageOperationService(IPackageOperationContextService packageOperationContextService, ILogger logger, INuGetPackageManager nuGetPackageManager,
            IRepositoryService repositoryService, IApiPackageRegistry apiPackageRegistry, IDefaultExtensibleProjectProvider defaultExtensibleProjectProvider,
            ISourceRepositoryProvider sourceRepositoryProvider)
        {
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => nuGetPackageManager);
            Argument.IsNotNull(() => repositoryService);
            //Argument.IsNotNull(() => repositoryCacheService);
            Argument.IsNotNull(() => apiPackageRegistry);
            Argument.IsNotNull(() => sourceRepositoryProvider);
            Argument.IsNotNull(() => defaultExtensibleProjectProvider);

            _packageOperationContextService = packageOperationContextService;
            _logger = logger;
             _nuGetPackageManager = nuGetPackageManager;
            //_repositoryCacheService = repositoryCacheService;
            _apiPackageRegistry = apiPackageRegistry;

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
        public void UninstallPackage(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);
            Argument.IsOfType(() => package, typeof (NuGetPackage));

            //var dependentsResolver = new DependentsWalker(_localRepository, null);

            //var walker = new UninstallWalker(_localRepository, dependentsResolver, null,
            //    _logger, true, false);

            try
            {
                var nuPackage = (NuGetPackage)package;
                //walker.ResolveOperations(nuGetPackage);

                //nuPackage should provide identity of installed package, which targeted for uninstall action
                using(var cts = new CancellationTokenSource())
                {
                    _nuGetPackageManager.UninstallPackageForProjectAsync(_defaultProject, nuPackage.Identity, cts.Token);
                }
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.Exceptions.Add(exception);
            }
        }

        public void InstallPackage(IPackageDetails package, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => package);
            Argument.IsOfType(() => package, typeof (NuGetPackage));

            var repository = _packageOperationContextService.CurrentContext.Repository;

            //var sourceRepository = _repositoryCacheService.GetNuGetRepository(repository);
            // var walker = new InstallWalker(_localRepository, sourceRepository, null, _logger, false, allowedPrerelease, DependencyVersion);

            try
            {
                ValidatePackage(package);
                //var nuGetPackage = EnsurePackageDependencies(((NuGetPackage)package).Package);
                //walker.ResolveOperations(nuGetPackage);
                var nuPackage = (NuGetPackage)package;

                //here was used a flag 'ignoreDependencies = false' and 'ignoreWalkInfo = false' in old code
                using (var cts = new CancellationTokenSource())
                {
                    _nuGetPackageManager.InstallPackageForProjectAsync(_defaultProject, nuPackage.Identity, cts.Token);
                }
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.Exceptions.Add(exception);
            }
        }

        public void UpdatePackages(IPackageDetails package, bool allowedPrerelease)
        {
            Argument.IsNotNull(() => package);
            Argument.IsOfType(() => package, typeof(NuGetPackage));

            try
            {
                ValidatePackage(package);
                //var nuGetPackage = EnsurePackageDependencies(((NuGetPackage)package)); //todo deprecated?

                //where to get target version?
                //somehow we should get target version from package
                //nugetPackage should provide 'update' identity
                var nuPackage = (NuGetPackage)package;

                using (var cts = new CancellationTokenSource())
                {
                    _nuGetPackageManager.UpdatePackageForProjectAsync(_defaultProject, nuPackage.Id, nuPackage.Identity.Version, cts.Token);
                }
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.Exceptions.Add(exception);
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
