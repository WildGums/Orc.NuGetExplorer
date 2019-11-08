namespace Orc.NuGetExplorer.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;
    using NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Services;

    internal class SettingsViewModel : ViewModelBase
    {
        private readonly bool _reloadConfigOnInitialize;
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly INuGetExplorerInitializationService _initializationService;

        private SettingsViewModel()
        {
            Title = "Package source settings";
        }

        public SettingsViewModel(ExplorerSettingsContainer settings) : this()
        {
            Argument.IsNotNull(() => settings);
            Settings = settings;
        }

        public SettingsViewModel(IModelProvider<ExplorerSettingsContainer> settingsProvider) : this()
        {
            Argument.IsNotNull(() => settingsProvider);
            Settings = settingsProvider.Model;
        }

        public SettingsViewModel(bool loadFeedsFromConfig, IModelProvider<ExplorerSettingsContainer> settingsProvider, INuGetConfigurationService configurationService,
            INuGetExplorerInitializationService initializationService)
            : this(settingsProvider)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => initializationService);

            _reloadConfigOnInitialize = loadFeedsFromConfig;
            _nuGetConfigurationService = configurationService;
            _initializationService = initializationService;
        }

        protected override Task InitializeAsync()
        {
            if (_reloadConfigOnInitialize)
            {
                LoadFeeds();
            }

            return base.InitializeAsync();
        }

        private void LoadFeeds()
        {
            var feeds = _nuGetConfigurationService.LoadPackageSources(false).OfType<NuGetFeed>().ToList();
            feeds.ForEach(feed => feed.Initialize());

            Settings.NuGetFeeds.AddRange(feeds);
        }

        [Model(SupportIEditableObject = false)]
        [Expose("NuGetFeeds")]
        public ExplorerSettingsContainer Settings { get; set; }

        protected override Task OnClosingAsync()
        {
            if (_reloadConfigOnInitialize)
            {
                Settings.Clear();
            }
            return base.OnClosingAsync();
        }

    }
}
