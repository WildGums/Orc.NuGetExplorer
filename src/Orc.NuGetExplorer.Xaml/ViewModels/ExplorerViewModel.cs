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
        private static bool _countingAndSearching;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackagesUpdatesSearcherService _packagesUpdatesSearcherService;
        private readonly IPackageBatchService _packageBatchService;
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
            OpenUpdateWindow = new TaskCommand(OnOpenUpdateWindowExecute, OnOpenUpdateWindowCanExecute);

            AccentColorHelper.CreateAccentColorResourceDictionary();
        }
        #endregion

        #region Properties
        [Model]
        [Expose("RepoCategories")]
        [Expose("SelectedRepository")]
        public RepositoryNavigator Navigator { get; private set; }

        [Model]
        [Expose("IsPrereleaseAllowed")]
        [Expose("SearchFilter")]
        [Expose("PackagesToSkip")]
        public SearchSettings SearchSettings { get; private set; }

        [Model]
        [Expose("TotalPackagesCount")]
        [Expose("PackageList")]
        public SearchResult SearchResult { get; private set; }

        public string ActionName { get; set; }
        public string FilterWatermark { get; set; }
        
        public ObservableCollection<IPackageDetails> AvailableUpdates { get; private set; }
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
            if (SearchResult.PackageList == null || Navigator.SelectedRepository == null)
            {
                return;
            }

            SearchResult.PackageList.Clear();

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

            if (_countingAndSearching || selectedRepository == null || SearchResult.PackageList == null)
            {
                return;
            }

            try
            {
                using (new DisposableToken(this, x => _countingAndSearching = true, x => _countingAndSearching = false))
                {
                    using (_pleaseWaitService.WaitingScope())
                    {
                        SearchSettings.PackagesToSkip = 0;

                        SearchResult.TotalPackagesCount = await _packageQueryService.CountPackagesAsync(selectedRepository, SearchSettings.SearchFilter, SearchSettings.IsPrereleaseAllowed ?? true);

                        var packageDetailses = await _packageQueryService.GetPackagesAsync(selectedRepository, SearchSettings.IsPrereleaseAllowed??true, SearchSettings.SearchFilter, SearchSettings.PackagesToSkip);
                        var packages = packageDetailses;

                        _dispatcherService.BeginInvoke(() =>
                        {
                            SearchResult.PackageList.Clear();
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

            await _packageCommandService.Execute(operation, package, Navigator.SelectedRepository, SearchSettings.IsPrereleaseAllowed ?? true);
            if (_packageCommandService.IsRefreshReqired(operation))
            {
                await CountAndSearch();
            }

            RefreshCanExecute();
        }

        private void RefreshCanExecute()
        {
            if (Navigator.SelectedRepository == null || SearchResult.PackageList == null)
            {
                return;
            }

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
                var packages = await _packagesUpdatesSearcherService.SearchForUpdatesAsync(SearchSettings.IsPrereleaseAllowed ?? true, false);

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