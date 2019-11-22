// --------------------------------------------------------------------------------------------------------------------
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
        public async Task UninstallPackageAsync(IPackageDetails package, CancellationToken token = default)
        {
            Argument.IsNotNull(() => package);

            try
            {
                //nuPackage should provide identity of installed package, which targeted for uninstall action
                await _nuGetPackageManager.UninstallPackageForProjectAsync(_defaultProject, package.GetIdentity(), token);
            }
            catch (Exception exception)
            {
                await _logger.LogAsync(LogLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.Exceptions.Add(exception);
            }
        }

        public async Task InstallPackageAsync(IPackageDetails package, bool allowedPrerelease = false, CancellationToken token = default)
        {
            Argument.IsNotNull(() => package);

            try
            {
                ValidatePackage(package);

                //repositories retrieved inside package manager now
                //todo use PackageOperationContextService instead on repositoryContextService
                
                //here was used a flag 'ignoreDependencies = false' and 'ignoreWalkInfo = false' in old code
                await _nuGetPackageManager.InstallPackageForProjectAsync(_defaultProject, package.GetIdentity(), token);
            }
            catch (Exception exception)
            {
                await _logger.LogAsync(LogLevel.Error, exception.Message);
                _packageOperationContextService.CurrentContext.Exceptions.Add(exception);
            }
        }


        public async Task UpdatePackagesAsync(IPackageDetails package, bool allowedPrerelease, CancellationToken token = default)
        {
            Argument.IsNotNull(() => package);

            try
            {
                ValidatePackage(package);

                //where to get target version?
                //somehow we should get target version from package
                //package should provide 'update' identity

                var updateIdentity = package.GetIdentity();

                await _nuGetPackageManager.UpdatePackageForProjectAsync(_defaultProject, updateIdentity.Id, updateIdentity.Version, token);
            }
            catch (Exception exception)
            {
                await _logger.LogAsync(LogLevel.Error, exception.Message);
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
