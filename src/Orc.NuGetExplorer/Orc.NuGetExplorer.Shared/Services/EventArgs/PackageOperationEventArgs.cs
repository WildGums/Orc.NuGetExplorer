// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.ComponentModel;
    using Catel;

    public class PackageOperationEventArgs : CancelEventArgs
    {
        #region Constructors
        internal PackageOperationEventArgs(IPackageDetails packageDetails, string installPath, PackageOperationType packageOperationType)
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
        #endregion
    }
}