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
        public PackageManagerWatcherBase(IPackageOperationNotificationService packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationNotificationService);

            packageOperationNotificationService.OperationStarted += OnOperationStarted;
            packageOperationNotificationService.OperationFinished += OnOperationFinished;
            packageOperationNotificationService.OperationsBatchStarted += OnOperationsBatchStarted;
            packageOperationNotificationService.OperationsBatchFinished += OnOperationsBatchFinished;
        }
        #endregion

        #region Methods
        protected virtual void OnOperationsBatchFinished(object sender, PackageOperationBatchEventArgs e)
        {
        }

        protected virtual void OnOperationsBatchStarted(object sender, PackageOperationBatchEventArgs e)
        {
        }

        protected virtual void OnOperationFinished(object sender, PackageOperationEventArgs e)
        {
        }

        protected virtual void OnOperationStarted(object sender, PackageOperationEventArgs e)
        {
        }
        #endregion
    }
}