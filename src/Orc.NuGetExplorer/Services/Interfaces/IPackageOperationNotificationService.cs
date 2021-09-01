namespace Orc.NuGetExplorer
{
    using System;

    public interface IPackageOperationNotificationService
    {
        #region Properties
        bool MuteAutomaticEvents { get; set; }
        #endregion

        #region Methods
        void NotifyOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages);
        [ObsoleteEx(ReplacementTypeOrMember = "Use NotifyOperationStarting(PackageOperationType, IPackageDetails) overload", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        void NotifyOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyOperationStarting(PackageOperationType operationType, IPackageDetails packageDetails);
        [ObsoleteEx(ReplacementTypeOrMember = "Use NotifyOperationFinished(PackageOperationType, IPackageDetails) overload", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyOperationFinished(PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyAutomaticOperationBatchStarting(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyAutomaticOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages);
        [ObsoleteEx(ReplacementTypeOrMember = "Use NotifyAutomaticOperationStarting(PackageOperationType, IPackageDetails) overload", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        void NotifyAutomaticOperationStarting(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyAutomaticOperationStarting(PackageOperationType operationType, IPackageDetails packageDetails);
        [ObsoleteEx(ReplacementTypeOrMember = "Use NotifyAutomaticOperationFinished(PackageOperationType, IPackageDetails) overload", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        void NotifyAutomaticOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyAutomaticOperationFinished(PackageOperationType operationType, IPackageDetails packageDetails);
        IDisposable DisableNotifications();
        #endregion

        #region Events
        event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarting;
        event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        event EventHandler<PackageOperationEventArgs> OperationStarting;
        event EventHandler<PackageOperationEventArgs> OperationFinished;
        #endregion
    }
}
