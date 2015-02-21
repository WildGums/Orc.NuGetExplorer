// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationItemsGroup.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Catel;

    internal class NavigationItemsGroup : NavigationItemBase
    {
        #region Constructors
        public NavigationItemsGroup(string name, params PackageSourcesNavigationItem[] packageSourcesNavigationItems)
            : base(name)
        {
            Argument.IsNotNull(() => packageSourcesNavigationItems);

            Children = new List<PackageSourcesNavigationItem>(packageSourcesNavigationItems);
        }
        #endregion

        #region Properties
        [DefaultValue(false)]
        public bool IsExpanded { get; set; }

        public int SelectedIndex { get; set; }
        public IList<PackageSourcesNavigationItem> Children { get; private set; }
        public PackageSourcesNavigationItem PackageSourceNavigationItem { get; private set; }
        #endregion

        #region Methods
        private void OnSelectedIndexChanged()
        {
            PackageSourceNavigationItem = Children[SelectedIndex];
        }
        #endregion
    }
}