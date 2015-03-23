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
    using NuGet;

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
        private IPackageRepository _packageRepository;
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

            AvailablePackages = new FastObservableCollection<PackageDetails>();
            AvailableUpdates = new ObservableCollection<IPackageDetails>();

            PackageAction = new TaskCommand<PackageDetails>(OnPackageActionExecute, OnPackageActionCanExecute);
            CheckForUpdates = new TaskCommand(OnCheckForUpdatesExecute);
            OpenUpdateWindow = new TaskCommand(OnOpenUpdateWindowExecute, OnOpenUpdateWindowCanExecute);
        }
        #endregion

        #region Properties
        public NamedRepository NamedRepository { get; set; }
        public string SearchFilter { get; set; }
        public PackageDetails SelectedPackage { get; set; }
        public FastObservableCollection<PackageDetails> AvailablePackages { get; private set; }
        public int TotalPackagesCount { get; set; }
        public int PackagesToSkip { get; set; }
        public string ActionName { get; set; }
        public ObservableCollection<IPackageDetails> AvailableUpdates { get; private set; }

        public string FilterWatermark
        {
            get
            {
                switch (NamedRepository.AllwedOperation)
                {
                    case PackageOperationType.Uninstall:
                        return "Search in Installed";

                    case PackageOperationType.Install:
                        return "Search Online";

                    case PackageOperationType.Update:
                        return "Search in Updates";
                }

                return "Search";
            }
        }

        public bool ShowUpdates
        {
            get
            {
                switch (NamedRepository.AllwedOperation)
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
                switch (NamedRepository.AllwedOperation)
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
                if (NamedRepository == null)
                {
                    IsPrereleaseAllowed = false;
                    return false;
                }

                switch (NamedRepository.AllwedOperation)
                {
                    case PackageOperationType.Uninstall:
                        return false;

                    case PackageOperationType.Install:
                    case PackageOperationType.Update:
                        return true;
                }
                // Blocking call!
                //return NamedRepository.Value.SupportsPrereleasePackages;
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

            if (NamedRepository != null)
            {
                ActionName = _packageCommandService.GetActionName(NamedRepository.AllwedOperation);
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

            if (NamedRepository == null)
            {
                return;
            }

            using (_pleaseWaitService.WaitingScope())
            {
                using (new DisposableToken(this, x => _updatingRepository = true, x => _updatingRepository = false))
                {
                    _packageRepository = NamedRepository.Value;
                    PackagesToSkip = 0;

                    TotalPackagesCount = await _packageQueryService.CountPackagesAsync(_packageRepository, SearchFilter, IsPrereleaseAllowed);
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

            if (NamedRepository != null)
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
        public TaskCommand<PackageDetails> PackageAction { get; private set; }

        private async Task OnPackageActionExecute(PackageDetails package)
        {
            var operation = NamedRepository.AllwedOperation;

            await _packageCommandService.Execute(operation, package, NamedRepository.Value, IsPrereleaseAllowed);
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
                package.IsActionExecuted = null;
                _packageCommandService.CanExecute(NamedRepository.AllwedOperation, package);
            }
        }

        private bool OnPackageActionCanExecute(PackageDetails parameter)
        {
            return _packageCommandService.CanExecute(NamedRepository.AllwedOperation, parameter);
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