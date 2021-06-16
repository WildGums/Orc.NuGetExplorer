// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationNotificationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
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
        IDisposable DisableNotifications();
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
        private bool _isNotificationsDisabled = false;

        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarting;
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        public event EventHandler<PackageOperationEventArgs> OperationStarting;
        public event EventHandler<PackageOperationEventArgs> OperationFinished;

        public bool MuteAutomaticEvents { get; set; }
        public bool IsNotificationsDisabled { get => _isNotificationsDisabled; private set => _isNotificationsDisabled = value; }

        public void NotifyOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            OperationsBatchStarting?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }

        public void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            OperationsBatchFinished?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages));
        }
        public void NotifyOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            OperationStarting?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            OperationFinished?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType));
        }

        public void NotifyAutomaticOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }
            OperationsBatchStarting?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages) { IsAutomatic = true });
        }

        public void NotifyAutomaticOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }
            OperationsBatchFinished?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages) { IsAutomatic = true });
        }

        public void NotifyAutomaticOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }
            OperationStarting?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType) { IsAutomatic = true });
        }

        public void NotifyAutomaticOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }
            OperationFinished?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType) { IsAutomatic = true });
        }

        public IDisposable DisableNotifications()
        {
            return new DisableNotificationToken(this);
        }

        private class DisableNotificationToken : DisposableToken<PackageOperationNotificationService>
        {
            public DisableNotificationToken(PackageOperationNotificationService instance) : this(instance, token => token.Instance.IsNotificationsDisabled = true,
                token => token.Instance.IsNotificationsDisabled = false, null)
            {

            }

            public DisableNotificationToken(PackageOperationNotificationService instance, Action<IDisposableToken<PackageOperationNotificationService>> initialize, Action<IDisposableToken<PackageOperationNotificationService>> dispose, object tag = null)
                : base(instance, initialize, dispose, tag)
            {
            }
        }
    }
}
