// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;

    internal class ExplorerViewModel : ViewModelBase
    {
        #region Fields
        private readonly IRepoNavigationService _repoNavigationService;
        #endregion

        #region Constructors
        public ExplorerViewModel(IRepoNavigationService repoNavigationService)
        {
            Argument.IsNotNull(() => repoNavigationService);

            _repoNavigationService = repoNavigationService;

            Navigator = _repoNavigationService.GetNavigator();
        }
        #endregion

        #region Properties
        [Model]
        [Expose("RepoCategories")]
        [Expose("SelectedRepoCategory")]
        public ReposNavigator Navigator { get; private set; }

        [ViewModelToModel("Navigator")]
        public NamedRepo SelectedNamedRepo { get; set; }
        #endregion

        protected override async Task Initialize()
        {
            await base.Initialize();

            // TODO: set selected named repo
        }
    }
}