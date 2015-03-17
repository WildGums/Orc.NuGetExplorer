// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Fody;
    using Catel.MVVM;
    using Catel.Services;
    using Models;

    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackagesUIService _packagesUiService;
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly INuGetFeedVerificationService _feedVerificationService;
        private readonly IMessageService _messageService;
        private readonly IPackagesUpdatesSearcherService _packagesUpdatesSearcherService;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public MainViewModel(IPackagesUIService packagesUiService, IEchoService echoService, INuGetConfigurationService nuGetConfigurationService,
            INuGetFeedVerificationService feedVerificationService, IMessageService messageService, IPackagesUpdatesSearcherService packagesUpdatesSearcherService,
            IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull(() => packagesUiService);
            Argument.IsNotNull(() => echoService);
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => feedVerificationService);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => pleaseWaitService);

            _packagesUiService = packagesUiService;
            _nuGetConfigurationService = nuGetConfigurationService;
            _feedVerificationService = feedVerificationService;
            _messageService = messageService;
            _packagesUpdatesSearcherService = packagesUpdatesSearcherService;
            _pleaseWaitService = pleaseWaitService;

            Echo = echoService.GetPackageManagementEcho();

            AvailableUpdates = new ObservableCollection<string>();

            ShowExplorer = new TaskCommand(OnShowExplorerExecute);
            AdddPackageSource = new TaskCommand(OnAdddPackageSourceExecute, OnAdddPackageSourceCanExecute);
            VerifyFeed = new TaskCommand(OnVerifyFeedExecute, OnVerifyFeedCanExecute);
            CheckForUpdates = new TaskCommand(OnCheckForUpdatesExecute);
        }
        #endregion

        #region Properties
        [Model]
        [Expose("Lines")]
        public PackageManagementEcho Echo { get; private set; }

        public bool? AllowPrerelease { get; set; }

        public string PackageSourceName { get; set; }
        public string PackageSourceUrl { get; set; }

        public ObservableCollection<string> AvailableUpdates { get; private set; }
        #endregion

        #region Commands
        public TaskCommand CheckForUpdates { get; private set; }

        private async Task OnCheckForUpdatesExecute()
        {
            AvailableUpdates.Clear();

            var packages = await _packagesUpdatesSearcherService.SearchForUpdatesAsync(AllowPrerelease, false);

            AvailableUpdates.AddRange(packages.Select(x => x.FullName).ToArray());
        }

        public TaskCommand AdddPackageSource { get; private set; }

        private async Task OnAdddPackageSourceExecute()
        {
            var packageSourceSaved = await _nuGetConfigurationService.SavePackageSourceAsync(PackageSourceName, PackageSourceUrl);
            if (!packageSourceSaved)
            {
                await _messageService.ShowWarning("Feed is invalid or unknown");
            }
        }

        private bool OnAdddPackageSourceCanExecute()
        {           
            return !string.IsNullOrWhiteSpace(PackageSourceName) && !string.IsNullOrWhiteSpace(PackageSourceUrl);
        }

        public TaskCommand VerifyFeed { get; private set; }

        private async Task OnVerifyFeedExecute()
        {
            var result = await _feedVerificationService.VerifyFeedAsync(PackageSourceUrl);
        }

        private bool OnVerifyFeedCanExecute()
        {
            return !string.IsNullOrWhiteSpace(PackageSourceUrl);
        }

        /// <summary>
        /// Gets the ShowExplorer command.
        /// </summary>
        public TaskCommand ShowExplorer { get; private set; }

        /// <summary>
        /// Method to invoke when the ShowExplorer command is executed.
        /// </summary>
        private async Task OnShowExplorerExecute()
        {
            await _packagesUiService.ShowPackagesExplorer();
        }
        #endregion
    }
}