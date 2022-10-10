namespace Orc.NuGetExplorer
{
    using System.ComponentModel;
    using Catel;

    public class PackageOperationBatchEventArgs : CancelEventArgs
    {
        #region Constructors
        internal PackageOperationBatchEventArgs(PackageOperationType operationType, params IPackageDetails[] packages)
        {
            Argument.IsNotNullOrEmptyArray(() => packages);

            Packages = packages;
            OperationType = operationType;
        }
        #endregion

        #region Properties
        public IPackageDetails[] Packages { get; private set; }
        public PackageOperationType OperationType { get; private set; }
        /// <summary>
        /// Determine is event raised by user actions or automatically
        /// </summary>
        public bool IsAutomatic { get; set; }
        #endregion
    }
}
