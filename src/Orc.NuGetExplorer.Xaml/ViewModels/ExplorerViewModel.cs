namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Catel;
    using Catel.Collections;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Models;
    using NuGetExplorer.Providers;

    internal class ExplorerViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const string DefaultStartPage = ExplorerPageName.Browse;
        private readonly IConfigurationService _configurationService;
        private readonly ITypeFactory _typeFactory;

        private readonly IDictionary<string, INuGetExplorerInitialState> _pageSetup = new Dictionary<string, INuGetExplorerInitialState>()
        {
            { ExplorerPageName.Browse, new NuGetExplorerInitialState(ExplorerTab.Browse, null) },
            { ExplorerPageName.Installed, new NuGetExplorerInitialState(ExplorerTab.Installed, null)},
            { ExplorerPageName.Updates, new NuGetExplorerInitialState(ExplorerTab.Update, null)}
        };

        private string _startPage = DefaultStartPage;


        public ExplorerViewModel(ICommandManager commandManager, ITypeFactory typeFactory,
            IModelProvider<ExplorerSettingsContainer> settingsProvider, IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => commandManager);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => typeFactory);

            _configurationService = configurationService;
            _typeFactory = typeFactory;

            CreateApplicationWideCommands(commandManager);

            if (settingsProvider is ExplorerSettingsContainerModelProvider settingsLazyProvider)
            {
                //force settings model re-initialization from config
                settingsLazyProvider.IsInitialized = false;
            }

            Settings = settingsProvider.Model;

            Title = "Package management";
        }

        //View to viewmodel
        public string StartPage { get; set; } = null;

        public ExplorerSettingsContainer Settings { get; set; }

        public IPackageSearchMetadata SelectedPackageMetadata { get; set; }

        public NuGetPackage SelectedPackageItem { get; set; }

        public INuGetExplorerInitialState BrowsePageParameters { get; set; }

        public INuGetExplorerInitialState InstalledPageParameters { get; set; }

        public INuGetExplorerInitialState UpdatesPageParameters { get; set; }

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

        protected override Task InitializeAsync()
        {
            InitializePages();

            return base.InitializeAsync();
        }

        protected override Task OnClosingAsync()
        {
            _configurationService.SetLastRepository("Browse", Settings.ObservedFeed.Name);

            Settings.Clear();

            return base.OnClosingAsync();
        }

        private void InitializePages()
        {
            BrowsePageParameters = _pageSetup[ExplorerPageName.Browse];
            InstalledPageParameters = _pageSetup[ExplorerPageName.Installed];
            UpdatesPageParameters = _pageSetup[ExplorerPageName.Updates];

            _pageSetup.Values.ForEach(page => 
                Pages.Add(_typeFactory.CreateInstanceWithParametersAndAutoCompletion<ExplorerPage>(page)));

            StartPage = _startPage;
        }

        private void CreateApplicationWideCommands(ICommandManager cm)
        {
            if (!cm.IsCommandCreated("RefreshCurrentPage"))
            {
                cm.CreateCommand("RefreshCurrentPage", new Catel.Windows.Input.InputGesture(Key.F5));
            }
        }
    }
}
