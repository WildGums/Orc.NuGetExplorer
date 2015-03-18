// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackageManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using NuGet;

    internal class NuGetPackageManager : PackageManager, INuGetPackageManager
    {
        private readonly IPackageCacheService _packageCacheService;

        #region Constructors
        public NuGetPackageManager(IPackageRepositoryService packageRepositoryService, INuGetConfigurationService nuGetConfigurationService,
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

        public NuGetPackageManager(IPackageRepository sourceRepository, string path)
            : base(sourceRepository, path)
        {
            this.PackageInstalling += (sender, args) => NotifyOperationStarted(_packageCacheService.GetPackageDetails(args.Package), args.InstallPath, PackageOperationType.Install);
            this.PackageInstalled += (sender, args) => NotifyOperationFinished(_packageCacheService.GetPackageDetails(args.Package), args.InstallPath, PackageOperationType.Install);

            this.PackageUninstalling += (sender, args) => NotifyOperationStarted(_packageCacheService.GetPackageDetails(args.Package), args.InstallPath, PackageOperationType.Uninstall);
            this.PackageUninstalled += (sender, args) => NotifyOperationFinished(_packageCacheService.GetPackageDetails(args.Package), args.InstallPath, PackageOperationType.Uninstall);
        }
        #endregion

        #region Methods
        public event EventHandler<NuGetOperationsBatchEventArgs> OperationsBatchStarted;
        public event EventHandler<NuGetOperationsBatchEventArgs> OperationsBatchFinished;
        public event EventHandler<NuGetPackageOperationEventArgs> OperationStarted;
        public event EventHandler<NuGetPackageOperationEventArgs> OperationFinished;

        public void NotifyOperationFinished(IPackageDetails packageDetails, string installPath, PackageOperationType operationType)
        {
            OperationFinished.SafeInvoke(this, new NuGetPackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationStarted(IPackageDetails packageDetails, string installPath, PackageOperationType operationType)
        {
            OperationStarted.SafeInvoke(this, new NuGetPackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationsBatchStarted(IPackageDetails packageDetails, PackageOperationType operationType)
        {
            OperationsBatchStarted.SafeInvoke(this, new NuGetOperationsBatchEventArgs(packageDetails, operationType));
        }

        public void NotifyOperationsBatchFinished(IPackageDetails packageDetails, PackageOperationType operationType)
        {
            OperationsBatchFinished.SafeInvoke(this, new NuGetOperationsBatchEventArgs(packageDetails, operationType));
        }
        #endregion
    }
}