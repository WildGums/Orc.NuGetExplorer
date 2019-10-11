using Orc.NuGetExplorer.Models;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.Services;

namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Models;
    using NuGetExplorer.Providers;
    using NuGetExplorer.Services;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class MainViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService _uIVisualizerService;
        private readonly NugetConfigurationService _configurationService;

        private readonly ITypeFactory _typeFactory;


        public MainViewModel(ITypeFactory typeFactory, IUIVisualizerService service, ICommandManager commandManager,
            IModelProvider<ExplorerSettingsContainer> settingsProvider, IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => service);
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => commandManager);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => configurationService);
            Argument.IsOfType(() => configurationService, typeof(NugetConfigurationService));

            _uIVisualizerService = service;
            _typeFactory = typeFactory;

            _configurationService = configurationService as NugetConfigurationService;

            CreateApplicationWideCommands(commandManager);

            Settings = settingsProvider.Model;
        }

        protected override Task InitializeAsync()
        {
            ExplorerPages = new ObservableCollection<ExplorerPageViewModel>();

            CreatePages();
            return base.InitializeAsync();
        }

        public ExplorerSettingsContainer Settings { get; set; }

        public IPackageSearchMetadata SelectedPackageMetadata { get; set; }

        private IViewModel _selectedPackageItem;
        public IViewModel SelectedPackageItem
        {
            get { return _selectedPackageItem; }
            set
            {
                _selectedPackageItem = null;
                RaisePropertyChanged(() => SelectedPackageItem);
                _selectedPackageItem = value;
                RaisePropertyChanged(() => SelectedPackageItem);

            }
        }

        public ObservableCollection<ExplorerPageViewModel> ExplorerPages { get; set; }

        private void CreatePages()
        {
            string[] pageNames = new string[] { "Browse", "Installed", "Updates" };

            for (int i = 0; i < pageNames.Length; i++)
            {
                var newPage = CreatePage(pageNames[i]);

                if (newPage != null)
                {
                    ExplorerPages.Add(newPage);
                }
            }

            ExplorerPageViewModel CreatePage(string title)
            {
                return _typeFactory.CreateInstanceWithParametersAndAutoCompletion<ExplorerPageViewModel>(Settings, title);
            }
        }

        protected override Task OnClosingAsync()
        {
            //update selected feed
            for (int i = 0; i < Settings.NuGetFeeds.Count; i++)
            {
                _configurationService.SetRoamingValueWithDefaultIdGenerator(Settings.NuGetFeeds[i]);
            }

            return base.OnClosingAsync();
        }

        private void CreateApplicationWideCommands(ICommandManager cm)
        {
            //move to initializer
            cm.CreateCommand("RefreshCurrentPage", new Catel.Windows.Input.InputGesture(Key.F5));
        }
    }
}
