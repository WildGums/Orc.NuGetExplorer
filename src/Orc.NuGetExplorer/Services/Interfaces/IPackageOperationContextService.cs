namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;

public interface IPackageOperationContextService
{
    IPackageOperationContext? CurrentContext { get; }

    event EventHandler<OperationContextEventArgs>? OperationContextDisposing;
    IDisposable UseOperationContext(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages);
}
