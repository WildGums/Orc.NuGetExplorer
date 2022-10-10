namespace Orc.NuGetExplorer
{
    using System.ComponentModel;
    using Catel;

    public class PackageOperationEventArgs : CancelEventArgs
    {
        #region Constructors
        internal PackageOperationEventArgs(IPackageDetails packageDetails, string installPath, PackageOperationType packageOperationType, bool isAutomatic = false)
        {
            Argument.IsNotNull(() => packageDetails);

            PackageDetails = packageDetails;
            InstallPath = installPath;
            PackageOperationType = packageOperationType;
        }
        #endregion

        #region Properties
        public string InstallPath { get; private set; }
        public PackageOperationType PackageOperationType { get; private set; }
        public IPackageDetails PackageDetails { get; private set; }
        /// <summary>
        /// Determine is event raised by user actions or automatically
        /// </summary>
        public bool IsAutomatic { get; set; }
        #endregion
    }
}
