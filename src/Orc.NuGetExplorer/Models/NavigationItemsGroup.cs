// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationItemsGroup.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Catel;

    using NuGet;

    internal class NavigationItemsGroup : NavigationItemBase
    {
        #region Constructors
        public NavigationItemsGroup(string name, AggregativeNavigationItem aggregativeNavigationItem)
            : base(name)
        {
            Argument.IsNotNull(() => aggregativeNavigationItem);

            Children = new List<NavigationItemBase> { aggregativeNavigationItem };
            Children.AddRange(aggregativeNavigationItem.PackageSources);
        }
        #endregion

        #region Properties
        [DefaultValue(false)]
        public bool IsExpanded { get; set; }

        public int SelectedIndex { get; set; }

        public IList<NavigationItemBase> Children { get; private set; }
        #endregion
    }
}