// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesBatch.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.ObjectModel;
    using Catel.Collections;

    public class PackagesBatch
    {
        #region Constructors
        public PackagesBatch()
        {
            PackageList = new FastObservableCollection<IPackageDetails>();
        }
        #endregion

        #region Properties
        public ObservableCollection<IPackageDetails> PackageList { get; set; }
        public PackageOperationType OperationType { get; set; }
        #endregion
    }
}