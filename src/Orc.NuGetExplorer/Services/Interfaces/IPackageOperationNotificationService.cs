// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageOperationNotificationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public interface IPackageOperationNotificationService
    {
        #region Events
        event EventHandler<PackageOperationBatchEventArgs> OperationsBatchStarted;
        event EventHandler<PackageOperationBatchEventArgs> OperationsBatchFinished;
        event EventHandler<PackageOperationEventArgs> OperationStarted;
        event EventHandler<PackageOperationEventArgs> OperationFinished;
        #endregion

        #region Methods
        void NotifyOperationBatchStarted(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyOperationStarted(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        #endregion
    }
}