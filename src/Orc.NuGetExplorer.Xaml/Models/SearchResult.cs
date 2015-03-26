// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel.Collections;
    using Catel.Data;

    internal class SearchResult : ModelBase
    {
        #region Constructors
        public SearchResult()
        {
            PackageList = new FastObservableCollection<IPackageDetails>();
        }
        #endregion

        #region Properties
        public int TotalPackagesCount { get; set; }
        public FastObservableCollection<IPackageDetails> PackageList { get; set; }
        #endregion
    }
}