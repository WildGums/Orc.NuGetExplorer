namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using Catel;
using Catel.IoC;
using Microsoft.Extensions.DependencyInjection;

internal class PackageOperationContextService : IPackageOperationContextService
{
    private readonly object _lockObject = new();
    private readonly IPackageOperationNotificationService _packageOperationNotificationService;
    private readonly IServiceProvider _serviceProvider;
    private PackageOperationContext? _rootContext;

    public PackageOperationContextService(IPackageOperationNotificationService packageOperationNotificationService,
        IServiceProvider serviceProvider)
    {
        _packageOperationNotificationService = packageOperationNotificationService;
        _serviceProvider = serviceProvider;
    }

    public IPackageOperationContext? CurrentContext { get; private set; }

    public event EventHandler<OperationContextEventArgs>? OperationContextDisposing;

    public IDisposable UseOperationContext(PackageOperationType operationType, IPackageDetails package)
    {
        return UseOperationContext(operationType, new[] { package });
    }

    public IDisposable UseOperationContext(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages)
    {
#pragma warning disable IDISP001 // Dispose created
        var context = ActivatorUtilities.CreateInstance<TemporaryFileSystemContext>(_serviceProvider);
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
                _packageOperationNotificationService.NotifyOperationBatchStarting(context.OperationType, context.Packages);
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
