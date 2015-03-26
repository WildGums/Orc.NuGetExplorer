// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System;
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

    internal class ExplorerViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static bool _searchingAndRefreshing;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageBatchService _packageBatchService;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPackagesUpdatesSearcherService _packagesUpdatesSearcherService;
        private readonly IPleaseWaitService _pleaseWaitService;
        private bool _isPrereleaseAllowed;
        #endregion

        #region Constructors
        public ExplorerViewModel(IRepositoryNavigatorService repositoryNavigatorService, ISearchSettingsService searchSettingsService, IPackageCommandService packageCommandService,
            IPleaseWaitService pleaseWaitService, IPackageQueryService packageQueryService, ISearchResultService searchResultService, IDispatcherService dispatcherService,
            IPackagesUpdatesSearcherService packagesUpdatesSearcherService, IPackageBatchService packageBatchService)
        {
            Argument.IsNotNull(() => repositoryNavigatorService);
            Argument.IsNotNull(() => searchSettingsService);
            Argument.IsNotNull(() => packageCommandService);
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => searchResultService);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => packagesUpdatesSearcherService);
            Argument.IsNotNull(() => packageBatchService);

            _packageCommandService = packageCommandService;
            _pleaseWaitService = pleaseWaitService;
            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;
            _packagesUpdatesSearcherService = packagesUpdatesSearcherService;
            _packageBatchService = packageBatchService;

            SearchSettings = searchSettingsService.SearchSettings;
            SearchResult = searchResultService.SearchResult;
            Navigator = repositoryNavigatorService.Navigator;
            AvailableUpdates = new ObservableCollection<IPackageDetails>();

            PackageAction = new TaskCommand<IPackageDetails>(OnPackageActionExecute, OnPackageActionCanExecute);
            CheckForUpdates = new TaskCommand(OnCheckForUpdatesExecute);
            OpenUpdateWindow = new TaskCommand(OnOpenUpdateWindowExecute);

            AccentColorHelper.CreateAccentColorResourceDictionary();
        }
        #endregion

        #region Properties
        [Model]
        [Expose("RepoCategories")]
        [Expose("SelectedRepository")]
        public RepositoryNavigator Navigator { get; private set; }

        [Model]
        [Expose("SearchFilter")]
        [Expose("PackagesToSkip")]
        public SearchSettings SearchSettings { get; private set; }

        [ViewModelToModel("SearchSettings")]
        public bool? IsPrereleaseAllowed { get; set; }

        [Model]
        [Expose("TotalPackagesCount")]
        [Expose("PackageList")]
        public SearchResult SearchResult { get; private set; }

        public string ActionName { get; private set; }
        public string FilterWatermark { get; private set; }
        public bool ShowUpdates { get; private set; }
        public IPackageDetails SelectedPackage { get; set; }
        public ObservableCollection<IPackageDetails> AvailableUpdates { get; private set; }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            await base.Initialize();

            await SearchAndRefresh();
        }

        private async void OnIsPrereleaseAllowedChanged()
        {
            if (!_searchingAndRefreshing && IsPrereleaseAllowed != null)
            {
                _isPrereleaseAllowed = IsPrereleaseAllowed.Value;
            }

            await SearchAndRefresh();
        }

        private async void OnPackagesToSkipChanged()
        {
            await SearchAndRefresh();
        }

        private async Task SearchAndRefresh()
        {
            if (_searchingAndRefreshing || SearchResult.PackageList == null || Navigator.SelectedRepository == null)
            {
                return;
            }

            using (new DisposableToken(this, x => _searchingAndRefreshing = true, x => _searchingAndRefreshing = false))
            {
                SetFilterWatermark();
                SetShowUpdates();
                SetActionName();
                SetIsPrereleaseAllowed();
                await CountAndSearch();
                RefreshCanExecute();
            }
        }

        private void SetIsPrereleaseAllowed()
        {
            switch (Navigator.SelectedRepository.OperationType)
            {
                case PackageOperationType.Install:
                case PackageOperationType.Update:
                    IsPrereleaseAllowed = _isPrereleaseAllowed;
                    break;

                default:
                    IsPrereleaseAllowed = null;
                    break;
            }
        }

        private void SetActionName()
        {
            ActionName = _packageCommandService.GetActionName(Navigator.SelectedRepository.OperationType);
        }

        private async void OnSelectedRepositoryChanged()
        {
            await SearchAndRefresh();
        }

        private void SetShowUpdates()
        {
            switch (Navigator.SelectedRepository.OperationType)
            {
                case PackageOperationType.Uninstall:
                    ShowUpdates = false;
                    break;

                case PackageOperationType.Install:
                    ShowUpdates = false;
                    break;

                case PackageOperationType.Update:
                    ShowUpdates = true;
                    break;

                default:
                    ShowUpdates = false;
                    break;
            }
        }

        private void SetFilterWatermark()
        {
            const string defaultWatermark = "Search";

            if (Navigator.SelectedRepository == null)
            {
                FilterWatermark = defaultWatermark;
                return;
            }

            switch (Navigator.SelectedRepository.OperationType)
            {
                case PackageOperationType.Uninstall:
                    FilterWatermark = "Search in Installed";
                    break;

                case PackageOperationType.Install:
                    FilterWatermark = "Search Online";
                    break;

                case PackageOperationType.Update:
                    FilterWatermark = "Search in Updates";
                    break;

                default:
                    FilterWatermark = defaultWatermark;
                    break;
            }
        }

        private async void OnSearchFilterChanged()
        {
            await SearchAndRefresh();
        }

        [Time]
        private async Task CountAndSearch()
        {
            var selectedRepository = Navigator.SelectedRepository;

            try
            {
                _dispatcherService.BeginInvoke(() => SearchResult.PackageList.Clear());

                using (_pleaseWaitService.WaitingScope())
                {
                    SearchSettings.PackagesToSkip = 0;

                    SearchResult.TotalPackagesCount = await _packageQueryService.CountPackagesAsync(selectedRepository, SearchSettings.SearchFilter, IsPrereleaseAllowed ?? true);

                    var packageDetailses = await _packageQueryService.GetPackagesAsync(selectedRepository, IsPrereleaseAllowed ?? true, SearchSettings.SearchFilter, SearchSettings.PackagesToSkip);
                    var packages = packageDetailses;

                    _dispatcherService.BeginInvoke(() =>
                    {
                        using (SearchResult.PackageList.SuspendChangeNotifications())
                        {
                            SearchResult.PackageList.AddRange(packages);
                        }
                    });
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to search packages.");
            }
            finally
            {
                // TODO: this is hack. Need to fix it.
                Navigator.SelectedRepository = selectedRepository;
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

            await _packageCommandService.Execute(operation, package, Navigator.SelectedRepository, IsPrereleaseAllowed ?? true);
            if (_packageCommandService.IsRefreshReqired(operation))
            {
                await CountAndSearch();
            }

            RefreshCanExecute();
        }

        private void RefreshCanExecute()
        {
            foreach (var package in SearchResult.PackageList)
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

        #endregion
    }
}