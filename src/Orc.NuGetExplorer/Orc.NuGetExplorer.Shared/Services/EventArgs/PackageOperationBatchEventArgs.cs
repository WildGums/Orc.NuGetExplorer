// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageOperationBatchEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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
        #endregion
    }
}