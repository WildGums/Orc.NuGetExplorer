// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.MVVM;

    internal class ExplorerViewModel : ViewModelBase
    {
        #region Constructors
        public ExplorerViewModel(INavigationTreeService navigationTreeService)
        {
            Argument.IsNotNull(() => navigationTreeService);

            NavigationItems = new List<NavigationItemsGroup>(navigationTreeService.CreateNavigationGroups());
            SelectedGroup = NavigationItems.FirstOrDefault(x => x.IsSelected);
        }
        #endregion

        #region Properties
        public IList<NavigationItemsGroup> NavigationItems { get; private set; }
        public NavigationItemsGroup SelectedGroup { get; set; }
        public PackageSourcesNavigationItem SelectedPackageSource { get; set; }
        #endregion

        #region Methods
        private void OnSelectedGroupChanged()
        {
            var navigationItemsGroup = SelectedGroup;
            var index = navigationItemsGroup.SelectedIndex;
            SelectedPackageSource = navigationItemsGroup.PackageSources[index];
        }
        #endregion
    }
}