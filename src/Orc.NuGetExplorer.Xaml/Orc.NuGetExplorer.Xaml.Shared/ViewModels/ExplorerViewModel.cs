// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Configuration;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Scoping;
    using Catel.Services;
    using Catel.Threading;
    using MethodTimer;
    using Scopes;

    internal class ExplorerViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static bool _searchingAndRefreshing;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageBatchService _packageBatchService;
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private readonly IConfigurationService _configurationService;
        private readonly IRepositoryNavigatorService _repositoryNavigatorService;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPackagesUpdatesSearcherService _packagesUpdatesSearcherService;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public ExplorerViewModel(IRepositoryNavigatorService repositoryNavigatorService, ISearchSettingsService searchSettingsService, IPackageCommandService packageCommandService,
            IPleaseWaitService pleaseWaitService, IPackageQueryService packageQueryService, ISearchResultService searchResultService, IDispatcherService dispatcherService,
            IPackagesUpdatesSearcherService packagesUpdatesSearcherService, IPackageBatchService packageBatchService, INuGetConfigurationService nuGetConfigurationService,
            IConfigurationService configurationService)
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
            Argument.IsNotNull(() => nuGetConfigurationService);
            Argument.IsNotNull(() => configurationService);

            _repositoryNavigatorService = repositoryNavigatorService;
            _packageCommandService = packageCommandService;
            _pleaseWaitService = pleaseWaitService;
            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;
            _packagesUpdatesSearcherService = packagesUpdatesSearcherService;
            _packageBatchService = packageBatchService;
            _nuGetConfigurationService = nuGetConfigurationService;
            _configurationService = configurationService;

            SearchSettings = searchSettingsService.SearchSettings;
            SearchResult = searchResultService.SearchResult;

            AvailableUpdates = new ObservableCollection<IPackageDetails>();

            PackageAction = new TaskCommand<IPackageDetails>(OnPackageActionExecuteAsync, OnPackageActionCanExecute);
            CheckForUpdates = new TaskCommand(OnCheckForUpdatesExecute);
            OpenUpdateWindow = new TaskCommand(OnOpenUpdateWindowExecuteAsync);

            AccentColorHelper.CreateAccentColorResourceDictionary();
        }
        #endregion

        #region Properties
        [Model]
        [Expose("RepositoryCategories")]
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
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _repositoryNavigatorService.Initialize();

            Navigator = _repositoryNavigatorService.Navigator;

            if (!Navigator.Initialized)
            {
                Navigator.Initialize();
            }

            await SearchAndRefreshAsync();
        }

        private async void OnIsPrereleaseAllowedChanged()
        {
            if (!_searchingAndRefreshing && IsPrereleaseAllowed != null && Navigator.SelectedRepository != null)
            {
                _nuGetConfigurationService.SetIsPrereleaseAllowed(Navigator.SelectedRepository, IsPrereleaseAllowed.Value);
            }

            await SearchAndRefreshAsync();
        }

        private async void OnPackagesToSkipChanged()
        {
            await SearchAndRefreshAsync();
        }

        private async Task SearchAndRefreshAsync()
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
                await CountAndSearchAsync();
                RefreshCanExecute();
            }
        }

        private void SetIsPrereleaseAllowed()
        {
            switch (Navigator.SelectedRepository.OperationType)
            {
                case PackageOperationType.Install:
                case PackageOperationType.Update:
                    IsPrereleaseAllowed = _nuGetConfigurationService.GetIsPrereleaseAllowed(Navigator.SelectedRepository);
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
            if (_searchingAndRefreshing || SearchResult.PackageList == null || Navigator.SelectedRepository == null)
            {
                return;
            }

            // TODO: saving selected repo, must be moved to RepositoryNavigationViewModel.OnSelectedRepositoryChanged()
            var selectedRepository = Navigator.SelectedRepository;
            var selectedRepositoryCategory = Navigator.SelectedRepositoryCategory;

            if (selectedRepositoryCategory == null || selectedRepository == null)
            {
                return;
            }

            _configurationService.SetLastRepository(selectedRepositoryCategory, selectedRepository);

            await SearchAndRefreshAsync();
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
            await SearchAndRefreshAsync();
        }

        [Time]
        private async Task CountAndSearchAsync()
        {
            var selectedRepository = Navigator.SelectedRepository;

            try
            {
                _dispatcherService.BeginInvoke(() => SearchResult.PackageList.Clear());

                using (ScopeManager<AuthenticationScope>.GetScopeManager(selectedRepository.Source.GetSafeScopeName(), () => new AuthenticationScope()))
                {
                    using (_pleaseWaitService.WaitingScope())
                    {
                        var searchSettings = SearchSettings;
                        searchSettings.PackagesToSkip = 0;

                        SearchResult.TotalPackagesCount = await TaskHelper.Run(() => _packageQueryService.CountPackages(selectedRepository, searchSettings.SearchFilter, IsPrereleaseAllowed ?? true), true);

                        var packageDetails = await TaskHelper.Run(() => _packageQueryService.GetPackages(selectedRepository, IsPrereleaseAllowed ?? true, searchSettings.SearchFilter, searchSettings.PackagesToSkip), true);
                        var packages = packageDetails;

                        _dispatcherService.BeginInvoke(() =>
                        {
                            using (SearchResult.PackageList.SuspendChangeNotifications())
                            {
                                SearchResult.PackageList.AddRange(packages);
                            }
                        });
                    }
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

        private async Task OnPackageActionExecuteAsync(IPackageDetails package)
        {
            if (Navigator.SelectedRepository == null)
            {
                return;
            }

            var operation = Navigator.SelectedRepository.OperationType;

            await TaskHelper.Run(() => _packageCommandService.Execute(operation, package, Navigator.SelectedRepository, IsPrereleaseAllowed ?? true), true);

            if (_packageCommandService.IsRefreshRequired(operation))
            {
                await CountAndSearchAsync();
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
                var packages = await TaskHelper.Run(() => _packagesUpdatesSearcherService.SearchForUpdates(), true);

                // TODO: AddRange doesn't refresh button state. need to fix later
                AvailableUpdates = new ObservableCollection<IPackageDetails>(packages);
            }
            await OnOpenUpdateWindowExecuteAsync();
        }

        public TaskCommand OpenUpdateWindow { get; private set; }

        private async Task OnOpenUpdateWindowExecuteAsync()
        {
            if (AvailableUpdates == null)
            {
                return;
            }

            await TaskHelper.Run(() => _packageBatchService.ShowPackagesBatch(AvailableUpdates, PackageOperationType.Update), true);
        }

        #endregion
    }
}