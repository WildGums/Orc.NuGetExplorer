namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    public class OperationContextEventArgs : EventArgs
    {
        public OperationContextEventArgs(IPackageOperationContext packageOperationContext)
        {
            Argument.IsNotNull(() => packageOperationContext);

            PackageOperationContext = packageOperationContext;
        }

        public IPackageOperationContext PackageOperationContext { get; private set; }
    }
}
