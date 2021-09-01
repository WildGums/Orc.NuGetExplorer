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
    using Orc.NuGetExplorer.Management;

    public class PackageOperationNotificationService : IPackageOperationNotificationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private bool _isNotificationsDisabled = false;

        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarting;
        public event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        public event EventHandler<PackageOperationEventArgs> OperationStarting;
        public event EventHandler<PackageOperationEventArgs> OperationFinished;

        public bool MuteAutomaticEvents { get; set; }
        public bool IsNotificationsDisabled
        {
            get => _isNotificationsDisabled;
            private set => _isNotificationsDisabled = value;
        }

        public PackageOperationNotificationService(IExtensibleProjectLocator extensibleProjectLocator)
        {
            Argument.IsNotNull(() => extensibleProjectLocator);

            _extensibleProjectLocator = extensibleProjectLocator;
        }

        public void NotifyOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            RaiseOperationsBatchStarting(operationType, false, packages);
        }

        public void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (IsNotificationsDisabled)
            {
                return;
            }

            RaiseOperationsBatchFinished(operationType, false, packages);
        }

        public void NotifyOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            Notify(operationType, packageDetails, _extensibleProjectLocator.GetDefaultProject(), (operation) => RaiseOperationStarting(operation, isAutomatic: false));
        }

        public void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            Notify(operationType, packageDetails, _extensibleProjectLocator.GetDefaultProject(), (operation) => RaiseOperationFinished(operation, isAutomatic: false));
        }

        public void NotifyAutomaticOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }

            if (IsNotificationsDisabled)
            {
                return;
            }

            RaiseOperationsBatchStarting(operationType, true, packages);
        }

        public void NotifyAutomaticOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }

            if (IsNotificationsDisabled)
            {
                return;
            }

            RaiseOperationsBatchFinished(operationType, true, packages);
        }

        public void NotifyAutomaticOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }

            Notify(operationType, packageDetails, _extensibleProjectLocator.GetDefaultProject(), (operation) => RaiseOperationStarting(operation, isAutomatic: true));
        }

        public void NotifyAutomaticOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails)
        {
            if (MuteAutomaticEvents)
            {
                Log.Info($"{operationType} notification was muted by notification service");
                return;
            }

            Notify(operationType, packageDetails, _extensibleProjectLocator.GetDefaultProject(), (operation) => RaiseOperationFinished(operation, isAutomatic: true));
        }

        protected virtual bool Notify(PackageOperationType operationType, IPackageDetails packageDetails, IExtensibleProject project, Action<PackageOperation> raiseAction = null)
        {
            Argument.IsNotNull(() => packageDetails);

            if (IsNotificationsDisabled)
            {
                return false;
            }

            PackageOperation packageOperation = null;

            switch (operationType)
            {
                case PackageOperationType.Update:
                    {
                        // TODO: Should we create actual Install/Update here, or they created by different notify method
                        packageOperation = UpdatePackageOperation.CreateUpdatePackageOperation(null, null, project);
                        break;
                    }

                case PackageOperationType.Install:
                    {
                        packageOperation = PackageOperation.CreateInstallPackageOperation(packageDetails.GetIdentity(), null, project);
                        break;
                    }

                case PackageOperationType.Uninstall:
                    {
                        packageOperation = PackageOperation.CreateUninstallPackageOperation(packageDetails.GetIdentity(), project);
                        break;
                    }
                case PackageOperationType.None:
                    {
                        return false;
                    }
            }

            if (packageOperation is null)
            {
                throw new InvalidOperationException("Cannot get current package operation");
            }

            raiseAction?.Invoke(packageOperation);
            return true;
        }

        private void RaiseOperationsBatchStarting(PackageOperationType operationType, bool isAutomatic, params IPackageDetails[] packages)
        {
            OperationsBatchStarting?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages)
            {
                IsAutomatic = isAutomatic
            });
        }

        private void RaiseOperationsBatchFinished(PackageOperationType operationType, bool isAutomatic, params IPackageDetails[] packages)
        {
            OperationsBatchFinished?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages)
            {
                IsAutomatic = isAutomatic
            });
        }

        private void RaiseOperationStarting(PackageOperation packageOperation, bool isAutomatic)
        {
            OperationStarting?.Invoke(this, new PackageOperationEventArgs(packageOperation, isAutomatic));
        }

        private void RaiseOperationFinished(PackageOperation packageOperation, bool isAutomatic)
        {
            OperationFinished?.Invoke(this, new PackageOperationEventArgs(packageOperation, isAutomatic));
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
