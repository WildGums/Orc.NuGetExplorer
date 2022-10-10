namespace Orc.NuGetExplorer
{
    using System;

    public interface IPackageOperationContextService
    {
        IPackageOperationContext CurrentContext { get; }

        event EventHandler<OperationContextEventArgs> OperationContextDisposing;
        IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages);
    }
}
