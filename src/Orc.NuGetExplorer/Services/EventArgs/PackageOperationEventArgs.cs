namespace Orc.NuGetExplorer
{
    using System.ComponentModel;

    public class PackageOperationEventArgs : CancelEventArgs
    {
        internal PackageOperationEventArgs(IPackageDetails packageDetails, string installPath, PackageOperationType packageOperationType, bool isAutomatic = false)
        {
            ArgumentNullException.ThrowIfNull(packageDetails);

            PackageDetails = packageDetails;
            InstallPath = installPath;
            PackageOperationType = packageOperationType;
        }

        public string InstallPath { get; private set; }

        public PackageOperationType PackageOperationType { get; private set; }

        public IPackageDetails PackageDetails { get; private set; }

        /// <summary>
        /// Determine is event raised by user actions or automatically
        /// </summary>
        public bool IsAutomatic { get; set; }
    }
}
