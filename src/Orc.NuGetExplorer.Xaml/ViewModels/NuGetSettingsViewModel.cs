namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.IoC;
    using Catel.MVVM;
    using NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;

    internal class NuGetSettingsViewModel : ViewModelBase
    {
        private const string DefaultTitle = "Package source settings";

        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly IDefaultPackageSourcesProvider _defaultPackageSourcesProvider;

        public NuGetSettingsViewModel(IModelProvider<ExplorerSettingsContainer> settingsProvider, INuGetConfigurationService configurationService, IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
            : this(DefaultTitle, settingsProvider, configurationService, defaultPackageSourcesProvider)
        {

        }

        public NuGetSettingsViewModel(string title, IModelProvider<ExplorerSettingsContainer> settingsProvider,
            INuGetConfigurationService configurationService, IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
            : this(settingsProvider?.Model, configurationService, defaultPackageSourcesProvider)
        {
            Argument.IsNotNull(() => settingsProvider);

            Title = title ?? DefaultTitle;
        }

        public NuGetSettingsViewModel(ExplorerSettingsContainer settings, INuGetConfigurationService configurationService, IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
        {
            Argument.IsNotNull(() => defaultPackageSourcesProvider);
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => settings);

            _defaultPackageSourcesProvider = defaultPackageSourcesProvider;
            _nuGetConfigurationService = configurationService;

            Title = DefaultTitle;
            Settings = settings;

            var serviceLocator = this.GetServiceLocator();

            if (serviceLocator.IsTypeRegistered<INuGetConfigurationResetService>())
            {
                CanReset = true;
            }
        }


        [Model(SupportIEditableObject = false)]
        [Expose("NuGetFeeds")]
        public ExplorerSettingsContainer Settings { get; set; }

        public IEnumerable<IPackageSource> PackageSources { get; set; }

        public bool CanReset { get; set; }

        public string DefaultFeed { get; set; }

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
}
