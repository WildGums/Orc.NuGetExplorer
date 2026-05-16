namespace Orc.NuGetExplorer.Example.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel.Fody;
using Catel.MVVM;
using Catel.Services;
using Orc.NuGetExplorer.Services;

public class MainViewModel : FeaturedViewModelBase
{
    private readonly INuGetFeedVerificationService _feedVerificationService;
    private readonly IMessageService _messageService;
    private readonly ILanguageService _languageService;
    private readonly INuGetConfigurationService _nuGetConfigurationService;
    private readonly INuGetExplorerInitializationService _initializationService;
    private readonly IPackagesUIService _packagesUiService;
    private readonly IPackagesUpdatesSearcherService _packagesUpdatesSearcherService;
    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly INuGetProjectUpgradeService _nuGetProjectUpgradeService;

    public MainViewModel(IServiceProvider serviceProvider,
        INuGetExplorerInitializationService initializationService,
        IPackagesUIService packagesUiService,
        IEchoService echoService,
        INuGetConfigurationService nuGetConfigurationService,
        INuGetFeedVerificationService feedVerificationService,
        ILanguageService languageService,
        IMessageService messageService,
        IPackagesUpdatesSearcherService packagesUpdatesSearcherService,
        INuGetProjectUpgradeService nuGetProjectUpgradeService,
        IUIVisualizerService uiVisualizerService)
        : base(serviceProvider)
    {
        _initializationService = initializationService;
        _packagesUiService = packagesUiService;
        _nuGetConfigurationService = nuGetConfigurationService;
        _feedVerificationService = feedVerificationService;
        _languageService = languageService;
        _messageService = messageService;
        _packagesUpdatesSearcherService = packagesUpdatesSearcherService;
        _nuGetProjectUpgradeService = nuGetProjectUpgradeService;
        _uiVisualizerService = uiVisualizerService;

        Echo = echoService.GetPackageManagementEcho();

        AvailableUpdates = new ObservableCollection<IPackageDetails>();

        ShowExplorer = new TaskCommand(serviceProvider, OnShowExplorerExecuteAsync);
        AddPackageSource = new TaskCommand(serviceProvider, OnAddPackageSourceExecuteAsync, OnAddPackageSourceCanExecute);
        VerifyFeed = new TaskCommand(serviceProvider, OnVerifyFeedExecuteAsync, OnVerifyFeedCanExecute);
        CheckForUpdates = new TaskCommand(serviceProvider, OnCheckForUpdatesExecuteAsync);
        OpenUpdateWindow = new TaskCommand(serviceProvider, OnOpenUpdateWindowExecuteAsync, OnOpenUpdateWindowCanExecute);
        Settings = new TaskCommand(serviceProvider, OnSettingsExecuteAsync);

        Title = _languageService.GetRequiredString("NuGetExplorerExample_MainViewModel_Title");
    }

    [Model]
    [Expose("Lines")]
    public PackageManagementEcho Echo { get; private set; }

    public bool? AllowPrerelease { get; set; }
    public string PackageSourceName { get; set; }
    public string PackageSourceUrl { get; set; }
    public ObservableCollection<IPackageDetails> AvailableUpdates { get; private set; }

    protected async override Task InitializeAsync()
    {
        await _nuGetProjectUpgradeService.CheckCurrentConfigurationAndRunAsync();
    }

    #region Commands
    public TaskCommand Settings { get; private set; }

    private async Task OnSettingsExecuteAsync()
    {
        await _packagesUiService.ShowPackagesSourceSettingsAsync();
    }

    public TaskCommand OpenUpdateWindow { get; private set; }

    private async Task OnOpenUpdateWindowExecuteAsync()
    {
    }

    private bool OnOpenUpdateWindowCanExecute()
    {
        return AvailableUpdates.Any();
    }

    public TaskCommand CheckForUpdates { get; private set; }

    private async Task OnCheckForUpdatesExecuteAsync()
    {
        AvailableUpdates.Clear();

        using (var cts = new CancellationTokenSource())
        {
            var packages = await _packagesUpdatesSearcherService.SearchForUpdatesAsync(AllowPrerelease, false, cts.Token);

            // Note: AddRange doesn't refresh button state
            AvailableUpdates = new ObservableCollection<IPackageDetails>(packages);
        }
    }

    public TaskCommand AddPackageSource { get; private set; }

    private async Task OnAddPackageSourceExecuteAsync()
    {
        var packageSourceSaved = await Task.Run(() => _nuGetConfigurationService.SavePackageSource(PackageSourceName, PackageSourceUrl, verifyFeed: true));
        if (!packageSourceSaved)
        {
            await _messageService.ShowWarningAsync(_languageService.GetRequiredString("NuGetExplorerExample_MainViewModel_Warning_InvalidOrUnknownFeed"));
        }
    }

    private bool OnAddPackageSourceCanExecute()
    {
        return !string.IsNullOrWhiteSpace(PackageSourceName) && !string.IsNullOrWhiteSpace(PackageSourceUrl);
    }

    public TaskCommand VerifyFeed { get; private set; }

    private async Task OnVerifyFeedExecuteAsync()
    {
        await _feedVerificationService.VerifyFeedAsync(PackageSourceUrl);
    }

    private bool OnVerifyFeedCanExecute()
    {
        return !string.IsNullOrWhiteSpace(PackageSourceUrl);
    }

    /// <summary>
    /// Gets the ShowExplorer command.
    /// </summary>
    public TaskCommand ShowExplorer { get; private set; }

    /// <summary>
    /// Method to invoke when the ShowExplorer command is executed.
    /// </summary>
    private async Task OnShowExplorerExecuteAsync()
    {
        await _packagesUiService.ShowPackagesExplorerAsync();
    }
    #endregion
}
