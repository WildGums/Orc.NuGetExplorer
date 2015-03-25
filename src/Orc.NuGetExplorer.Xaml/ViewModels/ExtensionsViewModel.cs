// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using MethodTimer;

    internal class ExtensionsViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static bool _updatingRepository;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPackagesUpdatesSearcherService _packagesUpdatesSearcherService;
        private readonly IPackageBatchService _packageBatchService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPleaseWaitService _pleaseWaitService;
        private bool _isPrereleaseAllowed;
        private IRepository _packageRepository;
        #endregion

        #region Constructors
        public ExtensionsViewModel(IPackageQueryService packageQueryService, IDispatcherService dispatcherService,
            IPleaseWaitService pleaseWaitService, IPackageCommandService packageCommandService,
            IPackagesUpdatesSearcherService packagesUpdatesSearcherService, IPackageBatchService packageBatchService)
        {
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => packageCommandService);
            Argument.IsNotNull(() => packagesUpdatesSearcherService);
            Argument.IsNotNull(() => packageBatchService);

            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;
            _pleaseWaitService = pleaseWaitService;
            _packageCommandService = packageCommandService;
            _packagesUpdatesSearcherService = packagesUpdatesSearcherService;
            _packageBatchService = packageBatchService;

            AvailablePackages = new FastObservableCollection<IPackageDetails>();
            AvailableUpdates = new ObservableCollection<IPackageDetails>();

            PackageAction = new TaskCommand<IPackageDetails>(OnPackageActionExecute, OnPackageActionCanExecute);
            CheckForUpdates = new TaskCommand(OnCheckForUpdatesExecute);
            OpenUpdateWindow = new TaskCommand(OnOpenUpdateWindowExecute, OnOpenUpdateWindowCanExecute);
        }
        #endregion

        #region Properties
        public IRepository SelectedRepository { get; set; }
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

                if (SelectedRepository == null)
                {
                    return defaultWatermark;
                }

                switch (SelectedRepository.OperationType)
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
                if (SelectedRepository == null)
                {
                    return false;
                }

                switch (SelectedRepository.OperationType)
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
                if (SelectedRepository == null)
                {
                    return _isPrereleaseAllowed;
                }

                switch (SelectedRepository.OperationType)
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
                if (SelectedRepository == null)
                {
                    IsPrereleaseAllowed = false;
                    return false;
                }

                switch (SelectedRepository.OperationType)
                {
                    case PackageOperationType.Uninstall:
                        return false;

                    case PackageOperationType.Install:
                    case PackageOperationType.Update:
                        return true;
                }
                // Blocking call!
                //return SelectedRepository.Value.SupportsPrereleasePackages;
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
            await UpdateRepository();

            RefreshCanExecute();
        }

        private async void OnPackagesToSkipChanged()
        {
            await SearchAndRefreshPackages();
        }

        private async Task SearchAndRefreshPackages()
        {
            await Search();

            RefreshCanExecute();
        }

        private async void OnNamedRepositoryChanged()
        {
            AvailablePackages.Clear();

            if (SelectedRepository != null)
            {
                ActionName = _packageCommandService.GetActionName(SelectedRepository.OperationType);
            }
            await UpdateRepository();

            RefreshCanExecute();
        }

        private async void OnSearchFilterChanged()
        {
            await UpdateRepository();

            RefreshCanExecute();
        }

        [Time]
        private async Task UpdateRepository()
        {
            if (_updatingRepository)
            {
                return;
            }

            if (SelectedRepository == null)
            {
                return;
            }

            using (_pleaseWaitService.WaitingScope())
            {
                using (new DisposableToken(this, x => _updatingRepository = true, x => _updatingRepository = false))
                {
                    PackagesToSkip = 0;

                    TotalPackagesCount = await _packageQueryService.CountPackagesAsync(SelectedRepository, SearchFilter, IsPrereleaseAllowed);
                }

                await Search();
            }
        }

        [Time]
        private async Task Search()
        {
            if (_updatingRepository)
            {
                return;
            }

            if (SelectedRepository != null)
            {
                using (_pleaseWaitService.WaitingScope())
                {
                    var packages = await _packageQueryService.GetPackagesAsync(_packageRepository, IsPrereleaseAllowed, SearchFilter, PackagesToSkip);

                    _dispatcherService.BeginInvoke(() =>
                    {
                        using (AvailablePackages.SuspendChangeNotifications())
                        {
                            AvailablePackages.ReplaceRange(packages);
                        }
                    });
                }
            }
        }
        #endregion

        #region Commands
        public TaskCommand<IPackageDetails> PackageAction { get; private set; }

        private async Task OnPackageActionExecute(IPackageDetails package)
        {
            var operation = SelectedRepository.OperationType;

            await _packageCommandService.Execute(operation, package, SelectedRepository, IsPrereleaseAllowed);
            if (_packageCommandService.IsRefreshReqired(operation))
            {
                await Search();
            }

            RefreshCanExecute();
        }

        private void RefreshCanExecute()
        {
            foreach (var package in AvailablePackages)
            {
                package.IsInstalled = null;
                _packageCommandService.CanExecute(SelectedRepository.OperationType, package);
            }
        }

        private bool OnPackageActionCanExecute(IPackageDetails parameter)
        {
            if (SelectedRepository == null)
            {
                return false;
            }

            return _packageCommandService.CanExecute(SelectedRepository.OperationType, parameter);
        }

        public TaskCommand CheckForUpdates { get; private set; }

        private async Task OnCheckForUpdatesExecute()
        {
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
            await _packageBatchService.ShowPackagesBatchAsync(AvailableUpdates, PackageOperationType.Update);
        }

        private bool OnOpenUpdateWindowCanExecute()
        {
            return AvailableUpdates.Any();
        }
        #endregion
    }
}