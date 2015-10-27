// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Threading;
    using Models;

    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private readonly INuGetFeedVerificationService _feedVerificationService;
        private readonly IMessageService _messageService;
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly IPackageBatchService _packageBatchService;
        private readonly IPackagesUIService _packagesUiService;
        private readonly IPackagesUpdatesSearcherService _packagesUpdatesSearcherService;
        private readonly IUIVisualizerService _uiVisualizerService;
        #endregion

        #region Constructors
        public MainViewModel(IPackagesUIService packagesUiService, IEchoService echoService, INuGetConfigurationService nuGetConfigurationService,
            INuGetFeedVerificationService feedVerificationService, IMessageService messageService, IPackagesUpdatesSearcherService packagesUpdatesSearcherService,
            IPackageBatchService packageBatchService, IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => packagesUiService);
            Argument.IsNotNull(() => echoService);
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => feedVerificationService);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => packageBatchService);
            Argument.IsNotNull(() => uiVisualizerService);

            _packagesUiService = packagesUiService;
            _nuGetConfigurationService = nuGetConfigurationService;
            _feedVerificationService = feedVerificationService;
            _messageService = messageService;
            _packagesUpdatesSearcherService = packagesUpdatesSearcherService;
            _packageBatchService = packageBatchService;
            _uiVisualizerService = uiVisualizerService;

            Echo = echoService.GetPackageManagementEcho();

            AvailableUpdates = new ObservableCollection<IPackageDetails>();

            ShowExplorer = new Command(OnShowExplorerExecute);
            AdddPackageSource = new TaskCommand(OnAdddPackageSourceExecute, OnAdddPackageSourceCanExecute);
            VerifyFeed = new TaskCommand(OnVerifyFeedExecute, OnVerifyFeedCanExecute);
            CheckForUpdates = new TaskCommand(OnCheckForUpdatesExecute);
            OpenUpdateWindow = new TaskCommand(OnOpenUpdateWindowExecute, OnOpenUpdateWindowCanExecute);
            Settings = new TaskCommand(OnSettingsExecute);
        }
        #endregion

        #region Properties
        [Model]
        [Expose("Lines")]
        public PackageManagementEcho Echo { get; private set; }

        public bool? AllowPrerelease { get; set; }
        public string PackageSourceName { get; set; }
        public string PackageSourceUrl { get; set; }
        public ObservableCollection<IPackageDetails> AvailableUpdates { get; private set; }
        #endregion

        #region Commands
        public TaskCommand Settings { get; private set; }

        private async Task OnSettingsExecute()
        {
            _uiVisualizerService.ShowDialog<SettingsViewModel>();
        }

        public TaskCommand OpenUpdateWindow { get; private set; }

        private async Task OnOpenUpdateWindowExecute()
        {
            await TaskHelper.Run(() => _packageBatchService.ShowPackagesBatch(AvailableUpdates, PackageOperationType.Update), true);
        }

        private bool OnOpenUpdateWindowCanExecute()
        {
            return AvailableUpdates.Any();
        }

        public TaskCommand CheckForUpdates { get; private set; }

        private async Task OnCheckForUpdatesExecute()
        {
            AvailableUpdates.Clear();

            var packages = await TaskHelper.Run(() => _packagesUpdatesSearcherService.SearchForUpdates(AllowPrerelease, false), true);

            // TODO: AddRange doesn't refresh button state. need to fix later
            AvailableUpdates = new ObservableCollection<IPackageDetails>(packages);
        }

        public TaskCommand AdddPackageSource { get; private set; }

        private async Task OnAdddPackageSourceExecute()
        {
            var packageSourceSaved = await TaskHelper.Run(() => _nuGetConfigurationService.SavePackageSource(PackageSourceName, PackageSourceUrl, verifyFeed: true), true);
            if (!packageSourceSaved)
            {
                await _messageService.ShowWarningAsync("Feed is invalid or unknown");
            }
        }

        private bool OnAdddPackageSourceCanExecute()
        {
            return !string.IsNullOrWhiteSpace(PackageSourceName) && !string.IsNullOrWhiteSpace(PackageSourceUrl);
        }

        public TaskCommand VerifyFeed { get; private set; }

        private async Task OnVerifyFeedExecute()
        {
            await TaskHelper.Run(() => _feedVerificationService.VerifyFeed(PackageSourceUrl), true);
        }

        private bool OnVerifyFeedCanExecute()
        {
            return !string.IsNullOrWhiteSpace(PackageSourceUrl);
        }

        /// <summary>
        /// Gets the ShowExplorer command.
        /// </summary>
        public Command ShowExplorer { get; private set; }

        /// <summary>
        /// Method to invoke when the ShowExplorer command is executed.
        /// </summary>
        private void OnShowExplorerExecute()
        {
            _packagesUiService.ShowPackagesExplorer();
        }
        #endregion
    }
}