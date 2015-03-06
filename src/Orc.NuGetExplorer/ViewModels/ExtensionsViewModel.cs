// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using MethodTimer;
    using NuGet;
    using Repositories;

    internal class ExtensionsViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static bool _updatingRepository;
        private IPackageRepository _packageRepository;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageManager _packageManager;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public ExtensionsViewModel(IPackageQueryService packageQueryService, IDispatcherService dispatcherService,
            IPackageManager packageManager, IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => packageManager);
            Argument.IsNotNull(() => pleaseWaitService);

            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;
            _packageManager = packageManager;
            _pleaseWaitService = pleaseWaitService;

            AvailablePackages = new FastObservableCollection<PackageDetails>();

            PackageAction = new Command(OnPackageActionExecute);

            // (will be dynamically renamed)
            ActionName = "Action";
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
        public bool IsPrereleaseAllowed { get; set; }

        public bool IsPrereleaseSupports
        {
            get
            {
                if (NamedRepository == null)
                {
                    IsPrereleaseAllowed = false;
                    return false;
                }

                return NamedRepository.Value.SupportsPrereleasePackages;
            }
        }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            await base.Initialize();

            await Search();
        }

        private async void OnIsPrereleaseAllowedChanged()
        {
            var updateRepository = _packageRepository as UpdateRepository;
            if (updateRepository != null)
            {
                updateRepository.AllowPrerelease = IsPrereleaseAllowed;
            }

            await UpdateRepository();
        }

        private async void OnPackagesToSkipChanged()
        {
            await Search();
        }

        private async void OnNamedRepositoryChanged()
        {
            await UpdateRepository();
        }

        private async void OnSearchFilterChanged()
        {
            await UpdateRepository();
        }

        private async void OnActionNameChanged()
        {
            await UpdateRepository();
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

                    TotalPackagesCount = await _packageRepository.CountPackagesAsync(SearchFilter, IsPrereleaseAllowed);
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
        public Command PackageAction { get; private set; }

        private void OnPackageActionExecute()
        {
            /*int skip = 0;
            int take = 10;
            IEnumerable<IPackage> versionsOfPackage;
            List<IPackage> accumList = new List<IPackage>();
            do
            {
                versionsOfPackage = _packageQueryService.GetVersionsOfPackageAsync(_packageRepository, SelectedPackage.Package, IsPrereleaseAllowed, ref skip, take);
                accumList.AddRange(versionsOfPackage);
            } while (versionsOfPackage.Any());*/

            //UpdatePackages();
        }

        private void UninstallPackage()
        {
            _packageManager.UninstallPackage(SelectedPackage.Package, true, false);
        }

        private void InstallPackage()
        {
            _packageManager.InstallPackage(SelectedPackage.Package, false, IsPrereleaseAllowed);
        }

        private void UpdatePackages()
        {
            _packageManager.UpdatePackage(SelectedPackage.Package, true, IsPrereleaseAllowed);
        }
        #endregion
    }
}