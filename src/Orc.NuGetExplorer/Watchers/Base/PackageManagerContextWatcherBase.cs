namespace Orc.NuGetExplorer
{
    using System.Linq;

    public abstract class PackageManagerContextWatcherBase : PackageManagerWatcherBase
    {
        private readonly IPackageOperationContextService _packageOperationContextService;

        protected PackageManagerContextWatcherBase(IPackageOperationNotificationService packageOperationNotificationService, IPackageOperationContextService packageOperationContextService)
            : base(packageOperationNotificationService)
        {
            ArgumentNullException.ThrowIfNull(packageOperationContextService);

            _packageOperationContextService = packageOperationContextService;

            _packageOperationContextService.OperationContextDisposing += OnOperationContextDisposing;
        }

        public bool HasContextErrors => CurrentContext?.Exceptions?.Any() ?? false;

        public IPackageOperationContext? CurrentContext => _packageOperationContextService.CurrentContext;

        protected virtual void OnOperationContextDisposing(object? sender, OperationContextEventArgs e)
        {

        }
    }
}
