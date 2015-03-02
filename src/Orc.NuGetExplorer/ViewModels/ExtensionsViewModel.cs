// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Catel;
    using Catel.Collections;
    using Catel.MVVM;
    using Catel.Services;
    using NuGet;

    public class ExtensionsViewModel : ViewModelBase
    {
        #region Fields
        private static bool _updatingRepisitory;
        private IPackageRepository _packageRepository;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackagesManager _packagesManager;
        private readonly IPackageQueryService _packageQueryService;
        #endregion

        #region Constructors
        public ExtensionsViewModel(IPackageQueryService packageQueryService, IDispatcherService dispatcherService,
            IPackagesManager packagesManager)
        {
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => packagesManager);

            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;
            _packagesManager = packagesManager;

            AvailablePackages = new ObservableCollection<PackageDetails>();

            PackageAction = new Command(OnPackageActionExecute);

            Search();
        }
        #endregion

        #region Properties
        public NamedRepo NamedRepository { get; set; }
        public string SearchFilter { get; set; }
        public PackageDetails SelectedPackage { get; set; }
        public ObservableCollection<PackageDetails> AvailablePackages { get; private set; }
        public int TotalPackagesCount { get; set; }
        public int PackagesToSkip { get; set; }
        public string ActionName { get; set; }
        #endregion

        #region Methods
        private void OnPackagesToSkipChanged()
        {
            Search();
        }

        private void OnNamedRepositoryChanged()
        {
            UpdateRepository();
        }

        private void OnSearchFilterChanged()
        {
            UpdateRepository();
        }

        private void OnActionNameChanged()
        {
            UpdateRepository();
        }

        private void UpdateRepository()
        {
            if (_updatingRepisitory)
            {
                return;
            }

            using (new DisposableToken(this, x => _updatingRepisitory = true, x => _updatingRepisitory = false))
            {
                _packageRepository = NamedRepository.Value;
                PackagesToSkip = 0;
                TotalPackagesCount = _packageQueryService.GetPackagesCount(_packageRepository, SearchFilter);
            }

            Search();
        }

        private void Search()
        {
            if (_updatingRepisitory)
            {
                return;
            }

            if (NamedRepository != null)
            {
                _dispatcherService.BeginInvoke(() =>
                {
                    var packageDetails = _packageQueryService.GetPackages(_packageRepository, SearchFilter, PackagesToSkip).ToArray();
                    AvailablePackages.ReplaceRange(packageDetails);
                });
            }
        }
        #endregion

        #region Commands
        public Command PackageAction { get; private set; }

        private void OnPackageActionExecute()
        {
            _packagesManager.Uninstall(SelectedPackage.Package);
        }
        #endregion
    }
}