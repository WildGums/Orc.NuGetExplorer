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
        internal PackageOperationEventArgs(PackageOperation packageOperation, bool isAutomatic = false)
        {
            Argument.IsNotNull(() => packageOperation);

            Details = packageOperation;
            IsAutomatic = isAutomatic;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "Use PackageOperationEventArgs(PackageOperation, bool) overload", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        internal PackageOperationEventArgs(IPackageDetails packageDetails, string path, PackageOperationType packageOperationType, bool isAutomatic = false)
        {
            Argument.IsNotNull(() => packageDetails);

            PackageDetails = packageDetails;
            InstallPath = path;
            PackageOperationType = packageOperationType;
            IsAutomatic = isAutomatic;
        }
        #endregion

        #region Properties
        [ObsoleteEx(TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public string InstallPath { get; private set; }
        [ObsoleteEx(ReplacementTypeOrMember = "PackageOperation.NuGetProjectActionType/PackageOperationEventArgs.IsUpdate", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public PackageOperationType PackageOperationType { get; private set; }
        [ObsoleteEx(ReplacementTypeOrMember = "PackageOperationEventArgs.Details", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public IPackageDetails PackageDetails { get; private set; }
        public PackageOperation Details { get; private set; }

        /// <summary>
        /// Determine is event raised by user actions or automatically
        /// </summary>
        public bool IsAutomatic { get; set; }

        public bool IsUpdate => Details is UpdatePackageOperation;
        #endregion
    }
}
