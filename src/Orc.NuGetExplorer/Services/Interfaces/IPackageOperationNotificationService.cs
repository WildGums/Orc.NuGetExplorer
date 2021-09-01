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
}
