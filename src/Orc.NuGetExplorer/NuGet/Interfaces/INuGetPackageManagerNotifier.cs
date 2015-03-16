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
        event EventHandler<NuGetOperationsBatchEventArgs> OperationsBatchStarted;
        event EventHandler<NuGetOperationsBatchEventArgs> OperationsBatchFinished;
        event EventHandler<NuGetPackageOperationEventArgs> OperationStarted;
        event EventHandler<NuGetPackageOperationEventArgs> OperationFinished;
        #endregion

        #region Methods
        void NotifyOperationsBatchStarted(IPackageDetails packageDetails, PackageOperationType operationType);
        void NotifyOperationsBatchFinished(IPackageDetails packageDetails, PackageOperationType operationType);
        void NotifyOperationFinished(IPackageDetails packageDetails, string installPath, PackageOperationType operationType);
        void NotifyOperationStarted(IPackageDetails packageDetails, string installPath, PackageOperationType operationType);
        #endregion
    }
}