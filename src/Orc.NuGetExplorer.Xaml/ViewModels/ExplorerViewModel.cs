namespace Orc.NuGetExplorer.ViewModels
{
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

    internal class ExplorerViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configurationService;

        private readonly ITypeFactory _typeFactory;

        public ExplorerViewModel(ITypeFactory typeFactory, ICommandManager commandManager,
            IModelProvider<ExplorerSettingsContainer> settingsProvider, IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => commandManager);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => configurationService);

            _typeFactory = typeFactory;
            _configurationService = configurationService;

            CreateApplicationWideCommands(commandManager);

            Settings = settingsProvider.Model;

            Title = "Explorer";
        }

        protected override Task InitializeAsync()
        {
            ExplorerPages = new ObservableCollection<ExplorerPageViewModel>();

            CreatePages();
            return base.InitializeAsync();
        }

        public ExplorerSettingsContainer Settings { get; set; }

        public IPackageSearchMetadata SelectedPackageMetadata { get; set; }

        public NuGetPackage SelectedPackageItem { get; set; }

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
