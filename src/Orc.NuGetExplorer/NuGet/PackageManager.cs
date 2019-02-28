// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using NuGet;

    internal class PackageManager : NuGet.PackageManager, IPackageManager
    {
        #region Fields
        private readonly IPackageCacheService _packageCacheService;
        #endregion

        #region Constructors
        public PackageManager(IRepositoryService repositoryService, INuGetConfigurationService nuGetConfigurationService,
            ILogger logger, IPackageCacheService packageCacheService, IRepositoryCacheService repositoryCacheService)
            : this(repositoryService.GetSourceAggregateRepository(), repositoryCacheService, nuGetConfigurationService.GetDestinationFolder())
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => packageCacheService);

            _packageCacheService = packageCacheService;
            Logger = logger;
        }

        public PackageManager(IRepository sourceRepository, IRepositoryCacheService repositoryCacheService, string path)
            : base(repositoryCacheService.GetNuGetRepository(sourceRepository), path)
        {
            
            PackageInstalling += (sender, args) => NotifyOperationStarting(args.InstallPath, PackageOperationType.Install, _packageCacheService.GetPackageDetails(repositoryCacheService.GetNuGetRepository(sourceRepository), args.Package, true));
            PackageInstalled += (sender, args) => NotifyOperationFinished(args.InstallPath, PackageOperationType.Install, _packageCacheService.GetPackageDetails(repositoryCacheService.GetNuGetRepository(sourceRepository), args.Package, true));

            PackageUninstalling += (sender, args) => NotifyOperationStarting(args.InstallPath, PackageOperationType.Uninstall, _packageCacheService.GetPackageDetails(repositoryCacheService.GetNuGetRepository(sourceRepository), args.Package, true));
            PackageUninstalled += (sender, args) => NotifyOperationFinished(args.InstallPath, PackageOperationType.Uninstall, _packageCacheService.GetPackageDetails(repositoryCacheService.GetNuGetRepository(sourceRepository), args.Package, true));
        }
        #endregion

        #region Methods
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarting;
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        public event EventHandler<PackageOperationEventArgs> OperationStarting;
        public event EventHandler<PackageOperationEventArgs> OperationFinished;

        public void NotifyOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            OperationStarting?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            OperationFinished?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            Argument.IsNotNullOrEmptyArray(() => packages);

            OperationsBatchStarting?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }

        public void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            Argument.IsNotNullOrEmptyArray(() => packages);

            OperationsBatchFinished?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }
        #endregion
    }
}
