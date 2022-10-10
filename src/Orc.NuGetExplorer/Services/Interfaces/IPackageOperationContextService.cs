namespace Orc.NuGetExplorer
{
    using System;

    public interface IPackageOperationContextService
    {
        #region Properties
        IPackageOperationContext CurrentContext { get; }
        #endregion

        #region Methods
        event EventHandler<OperationContextEventArgs> OperationContextDisposing;
        IDisposable UseOperationContext(PackageOperationType operationType, params IPackageDetails[] packages);
        #endregion
    }
}