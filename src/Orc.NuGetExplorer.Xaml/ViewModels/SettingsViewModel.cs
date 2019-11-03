namespace Orc.NuGetExplorer.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;
    using NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;

    internal class SettingsViewModel : ViewModelBase
    {
        private readonly bool _reloadConfigOnInitialize;
        private readonly INuGetConfigurationService _nuGetConfigurationService;

        public SettingsViewModel(ExplorerSettingsContainer settings)
        {
            Argument.IsNotNull(() => settings);
            Settings = settings;
        }

        public SettingsViewModel(IModelProvider<ExplorerSettingsContainer> settingsProvider)
        {
            Argument.IsNotNull(() => settingsProvider);
            Settings = settingsProvider.Model;
        }

        public SettingsViewModel(bool loadFeedsFromConfig, IModelProvider<ExplorerSettingsContainer> settingsProvider, INuGetConfigurationService configurationService)
            : this(settingsProvider)
        {
            Argument.IsNotNull(() => configurationService);

            _reloadConfigOnInitialize = loadFeedsFromConfig;
            _nuGetConfigurationService = configurationService;
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

    }
}
