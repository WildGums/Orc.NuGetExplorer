// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageManagerWatcherBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    public abstract class PackageManagerWatcherBase
    {
        private IPackageOperationNotificationService _packageOperationNotificationService;

        #region Constructors
        protected PackageManagerWatcherBase(IPackageOperationNotificationService packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationNotificationService);
            
            _packageOperationNotificationService = packageOperationNotificationService;

            packageOperationNotificationService.OperationStarting += OnOperationStarting;
            packageOperationNotificationService.OperationFinished += OnOperationFinished;
            packageOperationNotificationService.OperationsBatchStarting += OnOperationsBatchStarting;
            packageOperationNotificationService.OperationsBatchFinished += OnOperationsBatchFinished;
        }
        #endregion

        #region Methods
        // Note: this is a temporary hotfix
        internal void UpdatePackageOperationNotificationService(IPackageOperationNotificationService packageOperationNotificationService)
        {
            _packageOperationNotificationService.OperationStarting -= OnOperationStarting;
            _packageOperationNotificationService.OperationFinished -= OnOperationFinished;
            _packageOperationNotificationService.OperationsBatchStarting -= OnOperationsBatchStarting;
            _packageOperationNotificationService.OperationsBatchFinished -= OnOperationsBatchFinished;

            _packageOperationNotificationService = packageOperationNotificationService;

            _packageOperationNotificationService.OperationStarting += OnOperationStarting;
            _packageOperationNotificationService.OperationFinished += OnOperationFinished;
            _packageOperationNotificationService.OperationsBatchStarting += OnOperationsBatchStarting;
            _packageOperationNotificationService.OperationsBatchFinished += OnOperationsBatchFinished;
        }


        protected virtual void OnOperationsBatchFinished(object sender, PackageOperationBatchEventArgs e)
        {
        }

        protected virtual void OnOperationsBatchStarting(object sender, PackageOperationBatchEventArgs e)
        {
        }

        protected virtual void OnOperationFinished(object sender, PackageOperationEventArgs e)
        {
        }

        protected virtual void OnOperationStarting(object sender, PackageOperationEventArgs e)
        {
        }
        #endregion
    }
}
