// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceNavigationItem.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal class PackageSourceNavigationItem : NavigationItemBase
    {
        #region Constructors
        public PackageSourceNavigationItem(PackageSource packageSource)
            : base(packageSource.Name)
        {
            PackageSource = packageSource;
        }
        #endregion

        #region Properties
        public PackageSource PackageSource { get; private set; }
        #endregion
    }
}