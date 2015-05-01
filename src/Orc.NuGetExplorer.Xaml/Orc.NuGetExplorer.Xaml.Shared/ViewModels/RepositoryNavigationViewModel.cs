// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigationViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.Fody;
    using Catel.MVVM;

    internal class RepositoryNavigationViewModel : ViewModelBase
    {
        #region Fields
        private readonly IConfigurationService _configurationService;
        private readonly IRepositoryNavigatorService _repositoryNavigatorService;
        #endregion

        #region Constructors
        public RepositoryNavigationViewModel(IRepositoryNavigatorService repositoryNavigatorService, IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => repositoryNavigatorService);
            Argument.IsNotNull(() => configurationService);

            _repositoryNavigatorService = repositoryNavigatorService;
            _configurationService = configurationService;
        }
        #endregion

        #region Properties
        [Model]
        [Expose("RepositoryCategories")]
        [Expose("SelectedRepository")]
        [Expose("SelectedRepositoryCategory")]
        public RepositoryNavigator Navigator { get; private set; }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            await base.Initialize();

            Navigator = _repositoryNavigatorService.Navigator;

            if (!Navigator.Initialized)
            {
                Navigator.Initialize();
            }

            var lastRepositoryCategory = _configurationService.GetLastRepositoryCategory();

            var selectedRepositoryCategory = Navigator.RepositoryCategories.FirstOrDefault(x => string.Equals(x.Name, lastRepositoryCategory));

            if (selectedRepositoryCategory == null)
            {
                return;
            }

            selectedRepositoryCategory.IsSelected = true;
        }

        private void OnSelectedRepositoryCategoryChanged()
        {
            var selectedRepositoryCategory = Navigator.SelectedRepositoryCategory;
            if (selectedRepositoryCategory == null)
            {
                return;
            }

            _configurationService.SetLastRepositoryCategory(selectedRepositoryCategory.Name);

            var lastRepositoryName = _configurationService.GetLastRepository(selectedRepositoryCategory);

            var repository = selectedRepositoryCategory.Repositories.FirstOrDefault(x => string.Equals(x.Name, lastRepositoryName));
            if (repository == null && selectedRepositoryCategory.Repositories.Any())
            {
                repository = selectedRepositoryCategory.Repositories.FirstOrDefault(x => string.Equals(x.Name, RepositoryName.All));
            }

            if (repository == null && selectedRepositoryCategory.Repositories.Any())
            {
                repository = selectedRepositoryCategory.Repositories.First();
            }

            if (repository == null)
            {
                return;
            }

            if (!Equals(Navigator.SelectedRepository, repository))
            {
                Navigator.SelectedRepository = repository;
            }
        }

        #endregion
    }
}