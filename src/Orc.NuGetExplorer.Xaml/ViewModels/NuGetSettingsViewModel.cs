namespace Orc.NuGetExplorer.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catel;
using Catel.Fody;
using Catel.MVVM;
using Catel.Services;
using Orc.NuGetExplorer.Providers;

internal class NuGetSettingsViewModel : FeaturedViewModelBase
{
    private readonly INuGetConfigurationService _nuGetConfigurationService;
    private readonly IDefaultPackageSourcesProvider _defaultPackageSourcesProvider;
    private readonly ILanguageService _languageService;

    public NuGetSettingsViewModel(IModelProvider<ExplorerSettingsContainer> settingsProvider, 
        INuGetConfigurationService configurationService, IDefaultPackageSourcesProvider defaultPackageSourcesProvider,
        ILanguageService languageService,
        IServiceProvider serviceProvider)
        : this(null, settingsProvider, configurationService, defaultPackageSourcesProvider, languageService, serviceProvider)
    {

    }

    public NuGetSettingsViewModel(string? title, IModelProvider<ExplorerSettingsContainer> settingsProvider,
        INuGetConfigurationService configurationService, IDefaultPackageSourcesProvider defaultPackageSourcesProvider,
        ILanguageService languageService,
        IServiceProvider serviceProvider)
        : this(settingsProvider?.Model ?? throw new ArgumentException("'model' cannot be null"), configurationService, defaultPackageSourcesProvider, languageService, serviceProvider)
    {
        Title = title ?? languageService.GetRequiredString("NuGetExplorer_NuGetSettingsViewModel_Title");
    }

    public NuGetSettingsViewModel(ExplorerSettingsContainer settings, 
        INuGetConfigurationService configurationService, 
        IDefaultPackageSourcesProvider defaultPackageSourcesProvider,
        ILanguageService languageService,
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _defaultPackageSourcesProvider = defaultPackageSourcesProvider;
        _nuGetConfigurationService = configurationService;
        _languageService = languageService;

        Title = _languageService.GetRequiredString("NuGetExplorer_NuGetSettingsViewModel_Title");
        Settings = settings;

        if (serviceProvider.IsRegistered<INuGetConfigurationResetService>())
        {
            CanReset = true;
        }

        PackageSources = new List<IPackageSource>();
    }


    [Model(SupportIEditableObject = false)]
    [Expose("NuGetFeeds")]
    public ExplorerSettingsContainer Settings { get; set; }

    public IEnumerable<IPackageSource> PackageSources { get; set; }

    public bool CanReset { get; set; }

    public string? DefaultFeed { get; set; }

    protected override Task InitializeAsync()
    {
        LoadFeeds();

        InitializeDefaultFeed();

        return base.InitializeAsync();
    }

    private void LoadFeeds()
    {
        var feeds = _nuGetConfigurationService.LoadPackageSources(false).OfType<NuGetFeed>().ToList();
        feeds.ForEach(feed => feed.Initialize());
        PackageSources = feeds;
    }

    private void InitializeDefaultFeed()
    {
        DefaultFeed = _defaultPackageSourcesProvider.DefaultSource ?? string.Empty;
    }

    protected override async Task<bool> SaveAsync()
    {
        InitializeDefaultFeed();

        FillSettings();

        return await base.SaveAsync();
    }

    private void FillSettings()
    {
        Settings.Clear();

        Settings.NuGetFeeds.AddRange(PackageSources.OfType<NuGetFeed>());
    }
}
