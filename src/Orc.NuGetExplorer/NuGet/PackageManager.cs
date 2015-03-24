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
        #region Fields
        private readonly IPackageCacheService _packageCacheService;
        #endregion

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

        public PackageManager(IRepository sourceRepository, string path)
            : base(sourceRepository.ToNuGetRepository(), path)
        {
            this.PackageInstalling += (sender, args) => NotifyOperationStarting(args.InstallPath, PackageOperationType.Install, _packageCacheService.GetPackageDetails(args.Package));
            this.PackageInstalled += (sender, args) => NotifyOperationFinished(args.InstallPath, PackageOperationType.Install, _packageCacheService.GetPackageDetails(args.Package));

            this.PackageUninstalling += (sender, args) => NotifyOperationStarting(args.InstallPath, PackageOperationType.Uninstall, _packageCacheService.GetPackageDetails(args.Package));
            this.PackageUninstalled += (sender, args) => NotifyOperationFinished(args.InstallPath, PackageOperationType.Uninstall, _packageCacheService.GetPackageDetails(args.Package));
        }
        #endregion

        #region Methods
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarting;
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        public event EventHandler<PackageOperationEventArgs> OperationStarting;
        public event EventHandler<PackageOperationEventArgs> OperationFinished;

        public void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            OperationFinished.SafeInvoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            OperationStarting.SafeInvoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            OperationsBatchStarting.SafeInvoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }

        public void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            OperationsBatchFinished.SafeInvoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }
        #endregion
    }
}