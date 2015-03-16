// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetPackageManagerNotifierExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    internal static class INuGetPackageManagerNotifierExtensions
    {
        #region Methods
        public static IDisposable OperationsBatchContext(this INuGetPackageManagerNotifier nuGetPackageManagerNotifier, PackageDetails packageDetails, PackageOperationType operationType)
        {
            Argument.IsNotNull(() => nuGetPackageManagerNotifier);

            return new DisposableToken<INuGetPackageManagerNotifier>(nuGetPackageManagerNotifier,
                token => token.Instance.NotifyOperationsBatchStarted(packageDetails, operationType),
                token => token.Instance.NotifyOperationsBatchFinished(packageDetails, operationType));
        }
        #endregion
    }
}