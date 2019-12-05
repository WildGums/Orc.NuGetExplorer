namespace Orc.NuGetExplorer.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Services;

    internal class SettingsViewModel : ViewModelBase
    {
        private const string DefaultTitle = "Package source settings";

        private readonly bool _reloadConfigOnInitialize;
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly INuGetExplorerInitializationService _initializationService;
        private readonly INuGetConfigurationResetService _nuGetConfigurationResetService;
        private readonly IDefaultPackageSourcesProvider _defaultPackageSourcesProvider;

        private SettingsViewModel(IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
        {
            Argument.IsNotNull(() => defaultPackageSourcesProvider);

            _defaultPackageSourcesProvider = defaultPackageSourcesProvider;

            var sl = this.GetServiceLocator();

            if (sl.IsTypeRegistered<INuGetConfigurationResetService>())
            {
                _nuGetConfigurationResetService = sl.ResolveType<INuGetConfigurationResetService>();
                CanReset = true;
            }

            Reset = new TaskCommand(OnResetExecuteAsync, OnResetCanExecute);
        }

        public SettingsViewModel(ExplorerSettingsContainer settings, IDefaultPackageSourcesProvider defaultPackageSourcesProvider) : this(defaultPackageSourcesProvider)
        {
            Argument.IsNotNull(() => settings);
            Settings = settings;

            Title = DefaultTitle;
        }

        public SettingsViewModel(IModelProvider<ExplorerSettingsContainer> settingsProvider, string title, IDefaultPackageSourcesProvider defaultPackageSourcesProvider) : this(defaultPackageSourcesProvider)
        {
            Argument.IsNotNull(() => settingsProvider);

            Settings = settingsProvider.Model;
            Title = title ?? DefaultTitle;
        }

        public SettingsViewModel(bool loadFeedsFromConfig, string title, IModelProvider<ExplorerSettingsContainer> settingsProvider,
            INuGetConfigurationService configurationService, INuGetExplorerInitializationService initializationService, 
            IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
            : this(settingsProvider, title, defaultPackageSourcesProvider)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => initializationService);

            _reloadConfigOnInitialize = loadFeedsFromConfig;
            _nuGetConfigurationService = configurationService;
            _initializationService = initializationService;
        }


        [Model(SupportIEditableObject = false)]
        [Expose("NuGetFeeds")]
        public ExplorerSettingsContainer Settings { get; set; }

        public bool CanReset { get; set; }

        public string DefaultFeed { get; set; }

        protected override Task InitializeAsync()
        {
            if (_reloadConfigOnInitialize)
            {
                LoadFeeds();
            }

            InitializeDefaultFeed();

            return base.InitializeAsync();
        }

        private void LoadFeeds()
        {
            var feeds = _nuGetConfigurationService.LoadPackageSources(false).OfType<NuGetFeed>().ToList();
            feeds.ForEach(feed => feed.Initialize());

            Settings.NuGetFeeds.AddRange(feeds);
        }

        private void InitializeDefaultFeed()
        {
            DefaultFeed = _defaultPackageSourcesProvider.DefaultSource  ?? string.Empty;
        }

        protected override async Task<bool> SaveAsync()
        {
            InitializeDefaultFeed();

            return await base.SaveAsync();
        }

        protected override Task OnClosingAsync()
        {
            if (_reloadConfigOnInitialize)
            {
                Settings.Clear();
            }
            return base.OnClosingAsync();
        }

        #region Commands
        public TaskCommand Reset { get; private set; }

        private async Task OnResetExecuteAsync()
        {
            await _nuGetConfigurationResetService.Reset();
        }

        private bool OnResetCanExecute()
        {
            return _nuGetConfigurationResetService != null;
        }
        #endregion

    }
}
