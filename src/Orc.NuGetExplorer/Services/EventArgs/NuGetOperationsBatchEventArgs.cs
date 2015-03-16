// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetOperationsBatchEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.ComponentModel;

    public class NuGetOperationsBatchEventArgs : CancelEventArgs
    {
        #region Constructors
        internal NuGetOperationsBatchEventArgs(IPackageDetails packageDetails, PackageOperationType operationType)
        {
            PackageDetails = packageDetails;
            OperationType = operationType;
        }
        #endregion

        #region Properties
        public IPackageDetails PackageDetails { get; private set; }
        public PackageOperationType OperationType { get; private set; }
        #endregion
    }
}