namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using Catel;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class PackageOperationNotificationService : IPackageOperationNotificationService
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(PackageOperationNotificationService));

    private bool _isNotificationsDisabled = false;

    public event EventHandler<PackageOperationBatchEventArgs>? OperationsBatchStarting;
    public event EventHandler<PackageOperationBatchEventArgs>? OperationsBatchFinished;
    public event EventHandler<PackageOperationEventArgs>? OperationStarting;
    public event EventHandler<PackageOperationEventArgs>? OperationFinished;

    public bool MuteAutomaticEvents { get; set; }
    public bool IsNotificationsDisabled { get => _isNotificationsDisabled; private set => _isNotificationsDisabled = value; }

    public void NotifyOperationBatchStarting(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages)
    {
        if (IsNotificationsDisabled)
        {
            return;
        }

        OperationsBatchStarting?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages));
    }

    public void NotifyOperationBatchFinished(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages)
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

    public void NotifyAutomaticOperationBatchStarting(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages)
    {
        if (IsNotificationsDisabled)
        {
            return;
        }

        if (MuteAutomaticEvents)
        {
            Logger.LogInformation($"{operationType} notification was muted by notification service");
            return;
        }

        OperationsBatchStarting?.Invoke(this, new PackageOperationBatchEventArgs(operationType, packages) { IsAutomatic = true });
    }

    public void NotifyAutomaticOperationBatchFinished(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages)
    {
        if (IsNotificationsDisabled)
        {
            return;
        }

        if (MuteAutomaticEvents)
        {
            Logger.LogInformation($"{operationType} notification was muted by notification service");
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
            Logger.LogInformation($"{operationType} notification was muted by notification service");
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
            Logger.LogInformation($"{operationType} notification was muted by notification service");
            return;
        }

        OperationFinished?.Invoke(this, new PackageOperationEventArgs(packageDetails, installPath, operationType) { IsAutomatic = true });
    }

    public IDisposable DisableNotifications()
    {
        return new DisableNotificationToken(this);
    }

    private sealed class DisableNotificationToken : DisposableToken<PackageOperationNotificationService>
    {
        public DisableNotificationToken(PackageOperationNotificationService instance)
            : this(instance, token => token.Instance.IsNotificationsDisabled = true, token => token.Instance.IsNotificationsDisabled = false, null)
        {
        }

        public DisableNotificationToken(PackageOperationNotificationService instance, Action<IDisposableToken<PackageOperationNotificationService>> initialize, Action<IDisposableToken<PackageOperationNotificationService>> dispose, object? tag = null)
            : base(instance, initialize, dispose, tag)
        {
        }
    }
}
