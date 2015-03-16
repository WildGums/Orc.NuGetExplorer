namespace Orc.NuGetExplorer
{
    using System;

    public interface INuGetPackageManagerNotifier
    {
        event EventHandler<NuGetOperationsBatchEventArgs> OperationsBatchStarted;
        event EventHandler<NuGetOperationsBatchEventArgs> OperationsBatchFinished;
        event EventHandler<NuGetPackageOperationEventArgs> OperationStarted;
        event EventHandler<NuGetPackageOperationEventArgs> OperationFinished;

        void NotifyOperationsBatchStarted(IPackageDetails packageDetails, PackageOperationType operationType);
        void NotifyOperationsBatchFinished(IPackageDetails packageDetails, PackageOperationType operationType);
        void NotifyOperationFinished(IPackageDetails packageDetails, string installPath, PackageOperationType operationType);
        void NotifyOperationStarted(IPackageDetails packageDetails, string installPath, PackageOperationType operationType);
    }
}