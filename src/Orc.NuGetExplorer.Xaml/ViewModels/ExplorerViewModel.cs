namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Catel;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.MVVM;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Models;
    using NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Services;

    internal class ExplorerViewModel : ViewModelBase
    {
        private const string DefaultStartPage = ExplorerPageName.Browse;
        private readonly IConfigurationService _configurationService;
        private readonly INuGetExplorerInitializationService _initializationService;
        private readonly ITypeFactory _typeFactory;

        private readonly IDictionary<string, PackageSearchParameters> _pageSetup = new Dictionary<string, PackageSearchParameters>()
        {
            { ExplorerPageName.Browse, null},
            { ExplorerPageName.Installed, null},
            { ExplorerPageName.Updates, null }
        };

        private string _startPage = DefaultStartPage;


        public ExplorerViewModel(ITypeFactory typeFactory, ICommandManager commandManager,
            IModelProvider<ExplorerSettingsContainer> settingsProvider, IConfigurationService configurationService, INuGetExplorerInitializationService initializationService)
        {
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => commandManager);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => initializationService);

            _typeFactory = typeFactory;
            _configurationService = configurationService;
            _initializationService = initializationService;

            CreateApplicationWideCommands(commandManager);

            Settings = settingsProvider.Model;

            Title = "Package management";

            IsLogAutoScroll = true;
        }

        protected override Task InitializeAsync()
        {
            ExplorerPages = new ObservableCollection<ExplorerPageViewModel>();

            CreatePages();
            return base.InitializeAsync();
        }


        //View to viewmodel
        public string StartPage { get; set; } = DefaultStartPage;

        public ExplorerSettingsContainer Settings { get; set; }

        public IPackageSearchMetadata SelectedPackageMetadata { get; set; }

        public NuGetPackage SelectedPackageItem { get; set; }

        public ObservableCollection<ExplorerPageViewModel> ExplorerPages { get; set; }

        public bool IsLogAutoScroll { get; set; }

        public void ChangeStartPage(string name)
        {
            _startPage = name;   
        }

        public void ChangeInitialSearchParameters(string pagename, PackageSearchParameters searchParams)
        {
            if(_pageSetup.ContainsKey(pagename))
            {
                _pageSetup[pagename] = searchParams;
            }
        }

        private void CreatePages()
        {
            foreach(var page in _pageSetup)
            {
                var newPage = CreatePage(page.Key, page.Value);

                if (newPage != null)
                {
                    ExplorerPages.Add(newPage);
                }
            }

            StartPage = _startPage;

            ExplorerPageViewModel CreatePage(string title, PackageSearchParameters initialSearchParams)
            {
                return _typeFactory.CreateInstanceWithParametersAndAutoCompletion<ExplorerPageViewModel>(Settings, title, initialSearchParams);
            }
        }

        protected override Task OnClosingAsync()
        {
            _configurationService.SetLastRepository("Browse", Settings.ObservedFeed.Name);

            Settings.Clear();

            return base.OnClosingAsync();
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
