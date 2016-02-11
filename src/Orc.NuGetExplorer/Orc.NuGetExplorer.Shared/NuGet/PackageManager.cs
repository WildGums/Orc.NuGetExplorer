// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManager.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Runtime.CompilerServices;
    using Catel;
    using Catel.Logging;
    using Catel.Reflection;
    using NuGet;

    internal class PackageManager : NuGet.PackageManager, IPackageManager
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IPackageCacheService _packageCacheService;

        private readonly ConditionalWeakTable<IPackageDetails, IWeakEventListener> _packageEvents = new ConditionalWeakTable<IPackageDetails, IWeakEventListener>();
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
            PackageInstalling += (sender, args) => NotifyOperationStarting(args.InstallPath, PackageOperationType.Install, _packageCacheService.GetPackageDetails(args.Package));
            PackageInstalled += (sender, args) => NotifyOperationFinished(args.InstallPath, PackageOperationType.Install, _packageCacheService.GetPackageDetails(args.Package));

            PackageUninstalling += (sender, args) => NotifyOperationStarting(args.InstallPath, PackageOperationType.Uninstall, _packageCacheService.GetPackageDetails(args.Package));
            PackageUninstalled += (sender, args) => NotifyOperationFinished(args.InstallPath, PackageOperationType.Uninstall, _packageCacheService.GetPackageDetails(args.Package));
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

            //SubscribeToDownloadProgress(packageDetails);

            OperationStarting.SafeInvoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            //UnsubscribeFromDownloadProgress(packageDetails);

            OperationFinished.SafeInvoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            Argument.IsNotNullOrEmptyArray(() => packages);

            OperationsBatchStarting.SafeInvoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }

        public void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            Argument.IsNotNullOrEmptyArray(() => packages);

            OperationsBatchFinished.SafeInvoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }

        private void SubscribeToDownloadProgress(IPackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            var packageDownloader = GetPackageDownloaderInstance(packageDetails);
            if (packageDownloader == null)
            {
                return;
            }

            var weakEvent = this.SubscribeToWeakGenericEvent<ProgressEventArgs>(packageDownloader, "ProgressAvailable", OnPackageDownloadProgress);
            if (weakEvent != null)
            {
                _packageEvents.Add(packageDetails, weakEvent);
            }
        }

        private void UnsubscribeFromDownloadProgress(IPackageDetails packageDetails)
        {
            Argument.IsNotNull(() => packageDetails);

            IWeakEventListener weakEventListener;
            if (_packageEvents.TryGetValue(packageDetails, out weakEventListener))
            {
                weakEventListener.Detach();
                _packageEvents.Remove(packageDetails);
            }
        }

        private object GetPackageDownloaderInstance(IPackageDetails packageDetails)
        {
            var innerPackagePropertyInfo = packageDetails.GetType().GetPropertyEx("Package");
            if (innerPackagePropertyInfo == null)
            {
                return null;
            }

            var innerPackage = innerPackagePropertyInfo.GetValue(packageDetails, null);
            if (innerPackage == null)
            {
                return null;
            }

            var downloaderPropertyInfo = innerPackage.GetType().GetPropertyEx("Downloader");
            if (downloaderPropertyInfo == null)
            {
                return null;
            }

            var downloader = downloaderPropertyInfo.GetValue(innerPackage, null);
            if (downloader == null)
            {
                return null;
            }

            return downloader;
        }

        private void OnPackageDownloadProgress(object sender, ProgressEventArgs e)
        {
            Log.Debug("Progress of operation '{0}': {1} %", e.Operation, e.PercentComplete);
        }
        #endregion
    }
}