// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
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
