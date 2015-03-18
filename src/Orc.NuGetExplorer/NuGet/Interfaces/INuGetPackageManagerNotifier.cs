// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetPackageManagerNotifier.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public interface INuGetPackageManagerNotifier
    {
        #region Events
        event EventHandler<NuGetOperationBatchEventArgs> OperationsBatchStarted;
        event EventHandler<NuGetOperationBatchEventArgs> OperationsBatchFinished;
        event EventHandler<NuGetPackageOperationEventArgs> OperationStarted;
        event EventHandler<NuGetPackageOperationEventArgs> OperationFinished;
        #endregion

        #region Methods
        void NotifyOperationBatchStarted(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyOperationBatchFinished(PackageOperationType operationType, params IPackageDetails[] packages);
        void NotifyOperationFinished(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        void NotifyOperationStarted(string installPath, PackageOperationType operationType, IPackageDetails packageDetails);
        #endregion
    }
}