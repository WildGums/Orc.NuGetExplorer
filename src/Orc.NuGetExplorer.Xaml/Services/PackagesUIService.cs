namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Services;
    using ViewModels;

    internal class PackagesUIService : IPackagesUIService
    {
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly ITypeFactory _typeFactory;

        public PackagesUIService(IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => typeFactory);

            _uiVisualizerService = uiVisualizerService;
            _typeFactory = typeFactory;

            SettingsTitle = null;
        }

        /// <summary>
        /// Overriden title for settings window
        /// </summary>
        public string SettingsTitle { get; set; }

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
    }
}
