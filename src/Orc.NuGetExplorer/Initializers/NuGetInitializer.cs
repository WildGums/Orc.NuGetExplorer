// --------------------------------------------------------------------------------------------------------------------
// 
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    using Catel;
    using Catel.IoC;

    public class NuGetInitializer : INuGetInitializer
    {
        private readonly IServiceLocator _serviceLocator;

        public NuGetInitializer(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            _serviceLocator = serviceLocator;
        }

        public void Initialize()
        {
            if (_serviceLocator.IsTypeRegistered<IPackageOperationNotificationService>())
            {
                _serviceLocator.RemoveType(typeof(IPackageOperationNotificationService));
            }

            var nuGetPackageManager = _serviceLocator.ResolveType<IPackageManager>();
            _serviceLocator.RegisterInstance(typeof(IPackageOperationNotificationService), nuGetPackageManager);

            if (_serviceLocator.IsTypeRegistered<DeletemeWatcher>())
            {
                var deletemeWatcher = _serviceLocator.ResolveType<DeletemeWatcher>();
                deletemeWatcher.UpdatePackageOperationNotificationService(nuGetPackageManager);
            }
            else
            {
                _serviceLocator.RegisterTypeAndInstantiate<DeletemeWatcher>();
            }

            if (_serviceLocator.IsTypeRegistered<RollbackWatcher>())
            {
                var deletemeWatcher = _serviceLocator.ResolveType<RollbackWatcher>();
                deletemeWatcher.UpdatePackageOperationNotificationService(nuGetPackageManager);
            }
            else
            {
                _serviceLocator.RegisterTypeAndInstantiate<RollbackWatcher>();
            }
        }
    }
}
