namespace Orc.NuGetExplorer;

using System.Threading.Tasks;
using Catel.MVVM;
using Catel.Services;
using ViewModels;

internal class PackagesUIService : IPackagesUIService
{
    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly IViewModelFactory _viewModelFactory;

    public PackagesUIService(IUIVisualizerService uiVisualizerService, IViewModelFactory viewModelFactory)
    {
        _uiVisualizerService = uiVisualizerService;
        _viewModelFactory = viewModelFactory;

        SettingsTitle = string.Empty;
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
        var explorerVM = _viewModelFactory.CreateRequiredViewModel<ExplorerViewModel>(null);
        explorerVM.ChangeStartPage(initialState.Tab.Name);
        explorerVM.SetInitialPageParameters(initialState);

        await _uiVisualizerService.ShowDialogAsync(explorerVM);
    }

    public async Task<UIVisualizerResult?> ShowPackagesSourceSettingsAsync()
    {
        return await _uiVisualizerService.ShowDialogAsync<NuGetSettingsViewModel>(SettingsTitle);
    }
}
