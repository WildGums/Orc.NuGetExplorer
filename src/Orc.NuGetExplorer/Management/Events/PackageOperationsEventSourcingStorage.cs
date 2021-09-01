namespace Orc.NuGetExplorer.Management
{
    using Catel;

    public class PackageOperationsEventSourcingStorage
    {
        private readonly IPackageOperationNotificationService _packageOperationNotificationService;

        public PackageOperationsEventSourcingStorage(IPackageOperationNotificationService packageOperationNotificationService)
        {
            Argument.IsNotNull(() => packageOperationNotificationService);

            _packageOperationNotificationService = packageOperationNotificationService;
        }
    }
}
