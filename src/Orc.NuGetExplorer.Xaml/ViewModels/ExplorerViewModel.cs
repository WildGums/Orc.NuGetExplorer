namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Catel.Collections;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Models;
    using NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Configuration;
    using Orc.NuGetExplorer.Messaging;
    using Orc.NuGetExplorer.Services;

    internal class ExplorerViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const string DefaultStartPage = ExplorerPageName.Browse;
        private readonly IConfigurationService _configurationService;
        private readonly INuGetExplorerInitializationService _initializationService;
        private readonly ISettings _nuGetSettings;
        private readonly ITypeFactory _typeFactory;

        private readonly IDictionary<string, INuGetExplorerInitialState> _pageSetup = new Dictionary<string, INuGetExplorerInitialState>()
        {
            { ExplorerPageName.Browse, new NuGetExplorerInitialState(ExplorerTab.Browse, null) },
            { ExplorerPageName.Installed, new NuGetExplorerInitialState(ExplorerTab.Installed, null)},
            { ExplorerPageName.Updates, new NuGetExplorerInitialState(ExplorerTab.Update, null)}
        };

        private string _startPage = DefaultStartPage;

        public ExplorerViewModel(ITypeFactory typeFactory, ICommandManager commandManager, IModelProvider<ExplorerSettingsContainer> settingsProvider,
            IConfigurationService configurationService, INuGetExplorerInitializationService initializationService, ISettings nuGetSettings)
        {
            ArgumentNullException.ThrowIfNull(commandManager);
            ArgumentNullException.ThrowIfNull(settingsProvider);
            ArgumentNullException.ThrowIfNull(configurationService);
            ArgumentNullException.ThrowIfNull(typeFactory);

            _configurationService = configurationService;
            _initializationService = initializationService;
            _nuGetSettings = nuGetSettings;
            _typeFactory = typeFactory;

            CreateApplicationWideCommands(commandManager);

            if (settingsProvider is ExplorerSettingsContainerModelProvider settingsLazyProvider)
            {
                //force settings model re-initialization from config
                settingsLazyProvider.IsInitialized = false;
            }

            if (settingsProvider.Model is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Settings must be initialized first");
            }

            Settings = settingsProvider.Model;

            Title = "Package management";
        }

        // View to viewmodel
        public string? StartPage { get; set; } = null;

        public ExplorerSettingsContainer Settings { get; set; }

        public IPackageSearchMetadata? SelectedPackageMetadata { get; set; }

        public NuGetPackage? SelectedPackageItem { get; set; }

        public INuGetExplorerInitialState? BrowsePageParameters { get; set; }

        public INuGetExplorerInitialState? InstalledPageParameters { get; set; }

        public INuGetExplorerInitialState? UpdatesPageParameters { get; set; }

        public ObservableCollection<ExplorerPage> Pages { get; set; } = new ObservableCollection<ExplorerPage>();

        public void ChangeStartPage(string name)
        {
            _startPage = name;
        }

        public void SetInitialPageParameters(INuGetExplorerInitialState initialState)
        {
            var pagename = initialState.Tab.Name;

            if (string.IsNullOrEmpty(pagename))
            {
                Log.Error("Name for explorer page cannot be null or empty");
                return;
            }

            if (_pageSetup.ContainsKey(pagename))
            {
                _pageSetup[pagename] = initialState;
            }
        }

        protected override async Task InitializeAsync()
        {
            // Pages initializaiton
            BrowsePageParameters = _pageSetup[ExplorerPageName.Browse];
            InstalledPageParameters = _pageSetup[ExplorerPageName.Installed];
            UpdatesPageParameters = _pageSetup[ExplorerPageName.Updates];

            _pageSetup.Values.ForEach(page =>
                Pages.Add(_typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<ExplorerPage>(page)));

            foreach (var page in Pages)
            {
                page.PropertyChanged += OnExplorerPagePropertyChanged;
            }

            StartPage = _startPage;
        }

        protected override async Task CloseAsync()
        {
            foreach (var page in Pages)
            {
                page.PropertyChanged -= OnExplorerPagePropertyChanged;
            }
        }

        protected override Task OnClosingAsync()
        {
            _configurationService.SetLastRepository("Browse", Settings.ObservedFeed?.Name ?? string.Empty);
            _configurationService.SetIsPrereleaseIncluded(Settings.IsPreReleaseIncluded);
            _configurationService.SetIsHideInstalled(Settings.IsHideInstalled);

            if (_nuGetSettings is IVersionedSettings versionedSettings)
            {
                versionedSettings.UpdateVersion();
            }

            Settings.Clear();

            return base.OnClosingAsync();
        }

        private void OnExplorerPagePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.HasPropertyChanged(nameof(ExplorerPage.IsActive)))
            {
                SendIsActivePageChanged();
            }
        }

        private void SendIsActivePageChanged()
        {
            var activeTab = Pages.FirstOrDefault(p => p.IsActive)?.Parameters.Tab;
            if (activeTab is not null)
            {
                ActivatedExplorerTabMessage.SendWith(activeTab);
            }
        }

        // TODO: Provide a better way to create command (Don't hold gesture for whole application)
        private static void CreateApplicationWideCommands(ICommandManager cm)
        {
            if (!cm.IsCommandCreated("RefreshCurrentPage"))
            {
                cm.CreateCommand("RefreshCurrentPage", new Catel.Windows.Input.InputGesture(Key.F5));
            }
        }
    }
}
