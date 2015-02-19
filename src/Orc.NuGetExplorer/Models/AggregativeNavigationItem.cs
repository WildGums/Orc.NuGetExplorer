// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregativeNavigationItem.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    internal class AggregativeNavigationItem : NavigationItemBase
    {
        #region Constructors
        public AggregativeNavigationItem(string name, params PackageSourceNavigationItem[] packageSources)
            : base(name)
        {
            PackageSources = packageSources;
        }
        #endregion

        #region Properties
        public IList<PackageSourceNavigationItem> PackageSources { get; private set; }
        #endregion
    }
}