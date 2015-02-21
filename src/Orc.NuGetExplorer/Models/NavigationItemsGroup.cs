// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationItemsGroup.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Catel;

    public class NavigationItemsGroup : NavigationItemBase
    {
        #region Constructors
        public NavigationItemsGroup(string name, params PackageSourcesNavigationItem[] packageSourcesNavigationItems)
            : base(name)
        {
            Argument.IsNotNull(() => packageSourcesNavigationItems);

            PackageSources = new ObservableCollection<PackageSourcesNavigationItem>(packageSourcesNavigationItems);
        }
        #endregion

        #region Properties
        [DefaultValue(false)]
        public bool IsSelected { get; set; }

        public int SelectedIndex { get; set; }
        public IList<PackageSourcesNavigationItem> PackageSources { get; private set; }
        public PackageSourcesNavigationItem SelectedPackageSource { get; set; }
        
        #endregion

        #region Methods
        private void OnSelectedIndexChanged()
        {
            SelectedPackageSource = PackageSources[SelectedIndex];
        }
        #endregion
    }
}