﻿namespace Orc.NuGetExplorer
{
    public abstract class PackageManagerWatcherBase
    {
        protected PackageManagerWatcherBase(IPackageOperationNotificationService packageOperationNotificationService)
        {
            packageOperationNotificationService.OperationStarting += OnOperationStarting;
            packageOperationNotificationService.OperationFinished += OnOperationFinished;
            packageOperationNotificationService.OperationsBatchStarting += OnOperationsBatchStarting;
            packageOperationNotificationService.OperationsBatchFinished += OnOperationsBatchFinished;
        }

        protected virtual void OnOperationsBatchFinished(object? sender, PackageOperationBatchEventArgs e)
        {
        }

        protected virtual void OnOperationsBatchStarting(object? sender, PackageOperationBatchEventArgs e)
        {
        }

        protected virtual void OnOperationFinished(object? sender, PackageOperationEventArgs e)
        {
        }

        protected virtual void OnOperationStarting(object? sender, PackageOperationEventArgs e)
        {
        }
    }
}
