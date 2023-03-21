namespace Orc.NuGetExplorer;

using System;
using Catel;
using Catel.IoC;

internal class PackageOperationContextService : IPackageOperationContextService
{
    private readonly object _lockObject = new();
    private readonly IPackageOperationNotificationService _packageOperationNotificationService;
    private readonly ITypeFactory _typeFactory;
    private PackageOperationContext? _rootContext;

    public PackageOperationContextService(IPackageOperationNotificationService packageOperationNotificationService, ITypeFactory typeFactory)
    {
        ArgumentNullException.ThrowIfNull(packageOperationNotificationService);
        ArgumentNullException.ThrowIfNull(typeFactory);

        _packageOperationNotificationService = packageOperationNotificationService;
        _typeFactory = typeFactory;
    }

    public IPackageOperationContext? CurrentContext { get; private set; }

    public event EventHandler<OperationContextEventArgs>? OperationContextDisposing;

    public IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages)
    {
#pragma warning disable IDISP001 // Dispose created
        var context = _typeFactory.CreateRequiredInstance<TemporaryFileSystemContext>();
#pragma warning restore IDISP001 // Dispose created
        return new DisposableToken<PackageOperationContext>(new PackageOperationContext(packages, context)
            {
                OperationType = operationType,
            },
            token => ApplyOperationContext(token.Instance),
            token => CloseCurrentOperationContext(token.Instance));
    }

    private void ApplyOperationContext(PackageOperationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        lock (_lockObject)
        {
            if (_rootContext is null)
            {
                context.Exceptions?.Clear();

                _rootContext = context;
                CurrentContext = context;
                _packageOperationNotificationService.NotifyOperationBatchStarting(context.OperationType, context.Packages ?? new IPackageDetails[0]);
            }
            else
            {
                context.Parent = CurrentContext;
                CurrentContext = context;
            }
        }
    }

    private void CloseCurrentOperationContext(PackageOperationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        lock (_lockObject)
        {
            if (CurrentContext?.Parent is null)
            {
                OperationContextDisposing?.Invoke(this, new OperationContextEventArgs(context));
#pragma warning disable IDISP007 // Don't dispose injected.
                context.FileSystemContext.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.

                _packageOperationNotificationService.NotifyOperationBatchFinished(context.OperationType, context.Packages ?? new IPackageDetails[0]);
                _rootContext = null;
            }

            CurrentContext = CurrentContext?.Parent;
        }
    }
}