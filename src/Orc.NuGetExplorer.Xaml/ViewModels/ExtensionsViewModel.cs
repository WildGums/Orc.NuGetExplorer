// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using MethodTimer;

    internal class ExtensionsViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static bool _countingAndSearching;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageBatchService _packageBatchService;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPackagesUpdatesSearcherService _packagesUpdatesSearcherService;
        private readonly IPleaseWaitService _pleaseWaitService;
        private bool _isPrereleaseAllowed;
        #endregion

        #region Constructors
        public ExtensionsViewModel(IPackageQueryService packageQueryService, IDispatcherService dispatcherService,
            IPleaseWaitService pleaseWaitService, IPackageCommandService packageCommandService,
            IPackagesUpdatesSearcherService packagesUpdatesSearcherService, IPackageBatchService packageBatchService, IRepositoryNavigatorService repositoryNavigatorService)
        {
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageCommandService);
            Argument.IsNotNull(() => packagesUpdatesSearcherService);
            Argument.IsNotNull(() => packageBatchService);
            Argument.IsNotNull(() => repositoryNavigatorService);

            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;
            _pleaseWaitService = pleaseWaitService;
            _packageCommandService = packageCommandService;
            _packagesUpdatesSearcherService = packagesUpdatesSearcherService;
            _packageBatchService = packageBatchService;

            Navigator = repositoryNavigatorService.Navigator;

            AvailablePackages = new FastObservableCollection<IPackageDetails>();
            AvailableUpdates = new ObservableCollection<IPackageDetails>();

            PackageAction = new TaskCommand<IPackageDetails>(OnPackageActionExecute, OnPackageActionCanExecute);
            CheckForUpdates = new TaskCommand(OnCheckForUpdatesExecute);
            OpenUpdateWindow = new TaskCommand(OnOpenUpdateWindowExecute, OnOpenUpdateWindowCanExecute);
        }
        #endregion

        #region Properties
        [Model]
        [Expose("SelectedRepository")]
        [Expose("SelectedRepositoryCategory")]
        public RepositoryNavigator Navigator { get; set; }

        public string SearchFilter { get; set; }
        public IPackageDetails SelectedPackage { get; set; }
        public FastObservableCollection<IPackageDetails> AvailablePackages { get; private set; }
        public int TotalPackagesCount { get; set; }
        public int PackagesToSkip { get; set; }
        public string ActionName { get; set; }
        public ObservableCollection<IPackageDetails> AvailableUpdates { get; private set; }

        public string FilterWatermark
        {
            get
            {
                const string defaultWatermark = "Search";

                if (Navigator.SelectedRepository == null)
                {
                    return defaultWatermark;
                }

                switch (Navigator.SelectedRepository.OperationType)
                {
                    case PackageOperationType.Uninstall:
                        return "Search in Installed";

                    case PackageOperationType.Install:
                        return "Search Online";

                    case PackageOperationType.Update:
                        return "Search in Updates";
                }

                return defaultWatermark;
            }
        }

        public bool ShowUpdates
        {
            get
            {
                if (Navigator.SelectedRepository == null)
                {
                    return false;
                }

                switch (Navigator.SelectedRepository.OperationType)
                {
                    case PackageOperationType.Uninstall:
                        return false;

                    case PackageOperationType.Install:
                        return false;

                    case PackageOperationType.Update:
                        return true;
                }

                return false;
            }
        }

        public bool IsPrereleaseAllowed
        {
            get
            {
                if (Navigator.SelectedRepository == null)
                {
                    return _isPrereleaseAllowed;
                }

                switch (Navigator.SelectedRepository.OperationType)
                {
                    case PackageOperationType.Uninstall:
                        return true;

                    case PackageOperationType.Install:
                    case PackageOperationType.Update:
                        return _isPrereleaseAllowed;
                }

                return _isPrereleaseAllowed;
            }
            set
            {
                _isPrereleaseAllowed = value;
                OnIsPrereleaseAllowedChanged();
            }
        }

        public bool IsPrereleaseSupported
        {
            get
            {
                if (Navigator.SelectedRepository == null)
                {
                    IsPrereleaseAllowed = false;
                    return false;
                }

                switch (Navigator.SelectedRepository.OperationType)
                {
                    case PackageOperationType.Uninstall:
                        return false;

                    case PackageOperationType.Install:
                    case PackageOperationType.Update:
                        return true;
                }

                return false;
            }
        }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            await base.Initialize();

            await SearchAndRefreshPackages();
        }

        private async void OnIsPrereleaseAllowedChanged()
        {
            await SearchAndRefreshPackages();
        }

        private async void OnPackagesToSkipChanged()
        {
            await SearchAndRefreshPackages();
        }

        private async Task SearchAndRefreshPackages()
        {
            await CountAndSearch();

            RefreshCanExecute();
        }

        private async void OnSelectedRepositoryChanged()
        {
            if (AvailablePackages == null || Navigator.SelectedRepository == null)
            {
                return;
            }

            AvailablePackages.Clear();

            if (Navigator.SelectedRepository != null)
            {
                ActionName = _packageCommandService.GetActionName(Navigator.SelectedRepository.OperationType);
            }

            await SearchAndRefreshPackages();
        }

        private async void OnSearchFilterChanged()
        {
            await SearchAndRefreshPackages();
        }

        [Time]
        private async Task CountAndSearch()
        {
            var selectedRepository = Navigator.SelectedRepository;

            if (_countingAndSearching || selectedRepository == null || AvailablePackages == null)
            {
                return;
            }

            try
            {                
                using (new DisposableToken(this, x => _countingAndSearching = true, x => _countingAndSearching = false))
                {
                    using (_pleaseWaitService.WaitingScope())
                    {
                        PackagesToSkip = 0;

                        TotalPackagesCount = await _packageQueryService.CountPackagesAsync(selectedRepository, SearchFilter, IsPrereleaseAllowed);

                        var packageDetailses = await _packageQueryService.GetPackagesAsync(selectedRepository, IsPrereleaseAllowed, SearchFilter, PackagesToSkip);
                        var packages = packageDetailses;

                        _dispatcherService.BeginInvoke(() =>
                        {
                            AvailablePackages.Clear();
                            using (AvailablePackages.SuspendChangeNotifications())
                            {
                                AvailablePackages.AddRange(packages);
                            }
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to search packages.");
            }
        }

        #endregion

        #region Commands
        public TaskCommand<IPackageDetails> PackageAction { get; private set; }

        private async Task OnPackageActionExecute(IPackageDetails package)
        {
            if (Navigator.SelectedRepository == null)
            {
                return;
            }

            var operation = Navigator.SelectedRepository.OperationType;

            await _packageCommandService.Execute(operation, package, Navigator.SelectedRepository, IsPrereleaseAllowed);
            if (_packageCommandService.IsRefreshReqired(operation))
            {
                await CountAndSearch();
            }

            RefreshCanExecute();
        }

        private void RefreshCanExecute()
        {
            if (Navigator.SelectedRepository == null || AvailablePackages == null)
            {
                return;
            }

            foreach (var package in AvailablePackages)
            {
                package.IsInstalled = null;
                _packageCommandService.CanExecute(Navigator.SelectedRepository.OperationType, package);
            }
        }

        private bool OnPackageActionCanExecute(IPackageDetails parameter)
        {
            if (Navigator.SelectedRepository == null)
            {
                return false;
            }

            return _packageCommandService.CanExecute(Navigator.SelectedRepository.OperationType, parameter);
        }

        public TaskCommand CheckForUpdates { get; private set; }

        private async Task OnCheckForUpdatesExecute()
        {
            if (AvailableUpdates == null)
            {
                return;
            }

            AvailableUpdates.Clear();
            using (_pleaseWaitService.WaitingScope())
            {
                var packages = await _packagesUpdatesSearcherService.SearchForUpdatesAsync(IsPrereleaseAllowed, false);

                // TODO: AddRange doesn't refresh button state. neeed to fix later
                AvailableUpdates = new ObservableCollection<IPackageDetails>(packages);
            }
            await OnOpenUpdateWindowExecute();
        }

        public TaskCommand OpenUpdateWindow { get; private set; }

        private async Task OnOpenUpdateWindowExecute()
        {
            if (AvailableUpdates == null)
            {
                return;
            }

            await _packageBatchService.ShowPackagesBatchAsync(AvailableUpdates, PackageOperationType.Update);
        }

        private bool OnOpenUpdateWindowCanExecute()
        {
            if (AvailableUpdates == null)
            {
                return false;
            }

            return AvailableUpdates.Any();
        }
        #endregion
    }
}