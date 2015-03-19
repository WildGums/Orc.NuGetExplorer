// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using NuGet;

    internal class PackageManager : NuGet.PackageManager, IPackageManager
    {
        private readonly IPackageCacheService _packageCacheService;

        #region Constructors
        public PackageManager(IPackageRepositoryService packageRepositoryService, INuGetConfigurationService nuGetConfigurationService,
            ILogger logger, IPackageCacheService packageCacheService)
            : this(packageRepositoryService.GetSourceAggregateRepository(), nuGetConfigurationService.GetDestinationFolder())
        {
            Argument.IsNotNull(() => packageRepositoryService);
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => logger);
            Argument.IsNotNull(() => packageCacheService);
            
            _packageCacheService = packageCacheService;
            Logger = logger;
        }

        public PackageManager(IPackageRepository sourceRepository, string path)
            : base(sourceRepository, path)
        {
            this.PackageInstalling += (sender, args) => NotifyOperationStarted(args.InstallPath, PackageOperationType.Install, _packageCacheService.GetPackageDetails(args.Package));
            this.PackageInstalled += (sender, args) => NotifyOperationFinished(args.InstallPath, PackageOperationType.Install, _packageCacheService.GetPackageDetails(args.Package));

            this.PackageUninstalling += (sender, args) => NotifyOperationStarted(args.InstallPath, PackageOperationType.Uninstall, _packageCacheService.GetPackageDetails(args.Package));
            this.PackageUninstalled += (sender, args) => NotifyOperationFinished(args.InstallPath, PackageOperationType.Uninstall, _packageCacheService.GetPackageDetails(args.Package));
        }
        #endregion

        #region Methods
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarted;
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        public event EventHandler<PackageOperationEventArgs> OperationStarted;
        public event EventHandler<PackageOperationEventArgs> OperationFinished;

        public void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            OperationFinished.SafeInvoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationStarted(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            OperationStarted.SafeInvoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationBatchStarted(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            OperationsBatchStarted.SafeInvoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }

        public void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            OperationsBatchFinished.SafeInvoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }
        #endregion
    }
}