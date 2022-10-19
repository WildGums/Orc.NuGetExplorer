namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
            : this(settingsProvider?.Model ?? throw new ArgumentException("'model' cannot be null"), configurationService, defaultPackageSourcesProvider)
        {
            Title = title ?? DefaultTitle;
        }

        public NuGetSettingsViewModel(ExplorerSettingsContainer settings, INuGetConfigurationService configurationService, IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
        {
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(configurationService);
            ArgumentNullException.ThrowIfNull(defaultPackageSourcesProvider);

            _defaultPackageSourcesProvider = defaultPackageSourcesProvider;
            _nuGetConfigurationService = configurationService;

            Title = DefaultTitle;
            Settings = settings;

#pragma warning disable IDISP001 // Dispose created.
            var serviceLocator = this.GetServiceLocator();
#pragma warning restore IDISP001 // Dispose created.

            if (serviceLocator.IsTypeRegistered<INuGetConfigurationResetService>())
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
}
