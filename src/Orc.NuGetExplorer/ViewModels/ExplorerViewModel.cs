// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Linq;
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
        public ReposNavigator Navigator { get; private set; }

        [ViewModelToModel("Navigator")]
        public NamedRepository SelectedNamedRepository { get; set; }
        #endregion

        protected override async Task Initialize()
        {
            await base.Initialize();

            Navigator.SelectedRepositoryCategory = Navigator.RepoCategories.FirstOrDefault();
            var selectedRepositoryCategory = Navigator.SelectedRepositoryCategory;
            if (selectedRepositoryCategory != null)
            {
                selectedRepositoryCategory.IsSelected = true;
                SelectedNamedRepository = selectedRepositoryCategory.Repos.FirstOrDefault();
            }
        }
    }
}