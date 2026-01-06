namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using System.ComponentModel;
using Catel;

public class PackageOperationBatchEventArgs : CancelEventArgs
{
    internal PackageOperationBatchEventArgs(PackageOperationType operationType, IReadOnlyList<IPackageDetails> packages)
    {
        Packages = packages;
        OperationType = operationType;
    }

    public IReadOnlyList<IPackageDetails> Packages { get; private set; }

    public PackageOperationType OperationType { get; private set; }

    /// <summary>
    /// Determine is event raised by user actions or automatically
    /// </summary>
    public bool IsAutomatic { get; set; }
}
