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

    internal class PackageSourcesNavigationItem : NavigationItemBase
    {
        #region Constructors
        public PackageSourcesNavigationItem()
            : this(Enumerable.Empty<PackageSource>())
        {
        }

        public PackageSourcesNavigationItem(IEnumerable<PackageSource> packageSourceses)
            : base("All")
        {
            PackageSourceses = new List<PackageSource>(packageSourceses);
        }

        public PackageSourcesNavigationItem(PackageSource packageSource)
            : base(packageSource.Name)
        {
            PackageSourceses = new List<PackageSource> {packageSource};
        }
        #endregion

        #region Properties
        public IList<PackageSource> PackageSourceses { get; private set; }
        #endregion
    }
}