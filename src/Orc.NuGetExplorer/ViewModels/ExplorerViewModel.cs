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
        public ExplorerViewModel(IRepoNavigationFactory repoNavigationFactory)
        {
            Argument.IsNotNull(() => repoNavigationFactory);

            NavigationItems = new List<RepoCategory>(repoNavigationFactory.CreateRepoCategories());
            SelectedGroup = NavigationItems.FirstOrDefault(x => x.IsSelected);
        }
        #endregion

        #region Properties
        public IList<RepoCategory> NavigationItems { get; private set; }
        public RepoCategory SelectedGroup { get; set; }
        public NamedRepo SelectedPackageSource { get; set; }
        #endregion

        #region Methods
        private void OnSelectedGroupChanged()
        {
            var navigationItemsGroup = SelectedGroup;
            var index = navigationItemsGroup.SelectedIndex;
            SelectedPackageSource = navigationItemsGroup.Repos[index];
        }
        #endregion
    }
}