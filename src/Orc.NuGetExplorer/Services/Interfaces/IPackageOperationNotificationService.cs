// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationNotificationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel.Logging;

    public interface IPackageOperationNotificationService
    {
        #region Properties
        bool MuteAutomaticEvents { get; set; }
        #endregion

        #region Methods
        void NotifyOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyAutomaticOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyAutomaticOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyAutomaticOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyAutomaticOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        #endregion

        #region Events
        event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarting;
        event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        event EventHandler<PackageOperationEventArgs> OperationStarting;
        event EventHandler<PackageOperationEventArgs> OperationFinished;
        #endregion
    }

    public class PackageOperationNotificationService : IPackageOperationNotificationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarting;
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        public event EventHandler<PackageOperationEventArgs> OperationStarting;
        public event EventHandler<PackageOperationEventArgs> OperationFinished;

        public bool MuteAutomaticEvents { get; set; }

        public void NotifyOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            OperationsBatchStarting?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }

        public void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            OperationsBatchFinished?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }
        public void NotifyOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            OperationStarting?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            OperationFinished?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyAutomaticOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }
            OperationsBatchStarting?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages) { IsAutomatic = true });
        }

        public void NotifyAutomaticOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }
            OperationsBatchFinished?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages) { IsAutomatic = true }) ;
        }

        public void NotifyAutomaticOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }
            OperationStarting?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType) { IsAutomatic = true });
        }

        public void NotifyAutomaticOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }
            OperationFinished?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType) { IsAutomatic = true });
        }
    }

}
