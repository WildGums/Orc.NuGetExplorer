// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManagerWatcherBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    public abstract class PackageManagerWatcherBase
    {
        #region Constructors
        public PackageManagerWatcherBase(INuGetPackageManagerNotifier nuGetPackageManagerNotifier)
        {
            Argument.IsNotNull(() => nuGetPackageManagerNotifier);

            nuGetPackageManagerNotifier.OperationStarted += OnOperationStarted;
            nuGetPackageManagerNotifier.OperationFinished += OnOperationFinished;
            nuGetPackageManagerNotifier.OperationsBatchStarted += OnOperationsBatchStarted;
            nuGetPackageManagerNotifier.OperationsBatchFinished += OnOperationsBatchFinished;
        }
        #endregion

        #region Methods
        protected virtual void OnOperationsBatchFinished(object sender, NuGetOperationBatchEventArgs e)
        {
        }

        protected virtual void OnOperationsBatchStarted(object sender, NuGetOperationBatchEventArgs e)
        {
        }

        protected virtual void OnOperationFinished(object sender, NuGetPackageOperationEventArgs e)
        {
        }

        protected virtual void OnOperationStarted(object sender, NuGetPackageOperationEventArgs e)
        {
        }
        #endregion
    }
}