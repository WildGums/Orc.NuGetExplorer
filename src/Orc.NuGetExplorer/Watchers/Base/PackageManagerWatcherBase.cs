namespace Orc.NuGetExplorer
{
    using Catel;

    public abstract class PackageManagerWatcherBase
    {
        #region Constructors
        protected PackageManagerWatcherBase(IPackageOperationNotificationService packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationNotificationService);

            packageOperationNotificationService.OperationStarting += OnOperationStarting;
            packageOperationNotificationService.OperationFinished += OnOperationFinished;
            packageOperationNotificationService.OperationsBatchStarting += OnOperationsBatchStarting;
            packageOperationNotificationService.OperationsBatchFinished += OnOperationsBatchFinished;
        }
        #endregion

        #region Methods
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
