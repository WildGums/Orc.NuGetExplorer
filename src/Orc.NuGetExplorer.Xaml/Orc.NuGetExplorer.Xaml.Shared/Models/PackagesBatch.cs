﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesBatch.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.ObjectModel;
    using Catel.Collections;

    internal class PackagesBatch
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