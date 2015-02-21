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
        #region Fields
        private readonly INavigationTreeService _navigationTreeService;
        #endregion

        #region Constructors
        public ExplorerViewModel(INavigationTreeService navigationTreeService)
        {
            Argument.IsNotNull(() => navigationTreeService);

            _navigationTreeService = navigationTreeService;

            NavigationItems = new List<NavigationItemsGroup>(_navigationTreeService.CreateNavigationGroups());
            SelectedGroup = NavigationItems.FirstOrDefault(x => x.IsExpanded);

            GroupExpanded = new Command<NavigationItemsGroup>(OnGroupExpandedExecute);
        }
        #endregion

        #region Properties
        public IList<NavigationItemsGroup> NavigationItems { get; private set; }
        public NavigationItemsGroup SelectedGroup { get; set; }
        public PackageSourcesNavigationItem SelectedPackageSource { get; set; }
        #endregion

        #region Commands
        public Command<NavigationItemsGroup> GroupExpanded { get; private set; }

        private void OnGroupExpandedExecute(NavigationItemsGroup navigationItemsGroup)
        {
            SelectedGroup = navigationItemsGroup;
            var index = navigationItemsGroup.SelectedIndex;
            SelectedPackageSource = navigationItemsGroup.Children[index];
        }
        #endregion
    }
}