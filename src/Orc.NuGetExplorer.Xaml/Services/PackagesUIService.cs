namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Services;
    using Catel.Windows;
    using Orc.NuGetExplorer.Views;
    using ViewModels;

    internal class PackagesUIService : IPackagesUIService
    {
        #region Fields
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        public PackagesUIService(IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => typeFactory);

            _uiVisualizerService = uiVisualizerService;
            _typeFactory = typeFactory;

            SettingsTitle = null;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Overriden title for settings window
        /// </summary>
        public string SettingsTitle { get; set; }

        #endregion

        #region Methods
        public async Task ShowPackagesExplorerAsync()
        {
            await _uiVisualizerService.ShowDialogAsync<ExplorerViewModel>();
        }

        public async Task ShowPackagesExplorerAsync(INuGetExplorerInitialState initialState)
        {
            Argument.IsNotNull(() => initialState);

            var explorerVM = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<ExplorerViewModel>();
            explorerVM.ChangeStartPage(initialState.Tab.Name);
            explorerVM.SetInitialPageParameters(initialState);
            await _uiVisualizerService.ShowDialogAsync(explorerVM);
        }


        public async Task<bool?> ShowPackagesSourceSettingsAsync()
        {
            return await _uiVisualizerService.ShowDialogAsync<NuGetSettingsViewModel>(SettingsTitle);
        }

        public IEnumerable<DataWindow> GetOpenedPackageSourceWindows()
        {
            return System.Windows.Application.Current.Windows.OfType<NuGetSettingsWindow>();
        }

        public IEnumerable<DataWindow> GetOpenedPackageExplorerWindows()
        {
            return System.Windows.Application.Current.Windows.OfType<ExplorerWindow>();
        }

        #endregion
    }
}
