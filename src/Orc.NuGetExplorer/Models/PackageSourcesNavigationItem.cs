// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourcesNavigationItem.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using NuGet;

    public class PackageSourcesNavigationItem : NavigationItemBase
    {
        #region Constructors
        public PackageSourcesNavigationItem()
            : this(Enumerable.Empty<PackageSource>())
        {
        }

        public PackageSourcesNavigationItem(IEnumerable<PackageSource> packageSourceses)
            : base(PackageGroups.All)
        {
            PackageSources = new List<PackageSource>(packageSourceses);
        }

        public PackageSourcesNavigationItem(PackageSource packageSource)
            : base(packageSource.Name)
        {
            PackageSources = new List<PackageSource> {packageSource};
        }
        #endregion

        #region Properties
        public IList<PackageSource> PackageSources { get; private set; }
        #endregion
    }
}