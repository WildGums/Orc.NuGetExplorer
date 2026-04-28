namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;

public interface IPackageOperationNotificationService
{
    bool MuteAutomaticEvents { get; set; }

    void NotifyOperationBatchStarting(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages);
    void NotifyOperationBatchFinished(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages);
    void NotifyOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
    void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
    void NotifyAutomaticOperationBatchStarting(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages);
    void NotifyAutomaticOperationBatchFinished(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages);
    void NotifyAutomaticOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
    void NotifyAutomaticOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
    IDisposable DisableNotifications();

    event EventHandler<PackageOperationBatchEventArgs>? OperationsBatchStarting;
    event EventHandler<PackageOperationBatchEventArgs>? OperationsBatchFinished;
    event EventHandler<PackageOperationEventArgs>? OperationStarting;
    event EventHandler<PackageOperationEventArgs>? OperationFinished;
}
