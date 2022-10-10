﻿namespace Orc.NuGetExplorer
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
