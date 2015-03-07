// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackageOperationEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.ComponentModel;
    using Catel;

    public class NuGetPackageOperationEventArgs : CancelEventArgs
    {
        #region Constructors
        internal NuGetPackageOperationEventArgs(PackageDetails packageDetails, string installPath)
        {
            Argument.IsNotNull(() => packageDetails);
            Argument.IsNotNullOrWhitespace(() => installPath);

            PackageDetails = packageDetails;
            InstallPath = installPath;
        }
        #endregion

        #region Properties
        public string InstallPath { get; private set; }
        public IPackageDetails PackageDetails { get; private set; }
        #endregion
    }
}