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
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using NuGet;

    internal class ExplorerViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IRepositoryNavigationService _repositoryNavigationService;
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        public ExplorerViewModel(IRepositoryNavigationService repositoryNavigationService, ITypeFactory typeFactory, INuGetConfigurationService configurationService)
        {
            Argument.IsNotNull(() => repositoryNavigationService);
            Argument.IsNotNull(() => typeFactory);

            _repositoryNavigationService = repositoryNavigationService;
            _typeFactory = typeFactory;

            Navigator = _repositoryNavigationService.GetNavigator();

            HttpClient.DefaultCredentialProvider = _typeFactory.CreateInstance<NuGetSettingsCredentialProvider>();
        }
        #endregion

        #region Properties
        [Model]
        [Expose("RepoCategories")]
        public RepositoryNavigator Navigator { get; private set; }

        [ViewModelToModel("Navigator")]
        public NamedRepository SelectedNamedRepository { get; set; }
        #endregion

        #region Methods
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
        #endregion
    }
}