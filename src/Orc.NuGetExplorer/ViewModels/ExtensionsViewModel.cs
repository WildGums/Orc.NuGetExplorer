// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Catel;
    using Catel.Collections;
    using Catel.MVVM;
    using Catel.Services;
    using NuGet;
    using Repositories;

    public class ExtensionsViewModel : ViewModelBase
    {
        #region Fields
        private static bool _updatingRepisitory;
        private IPackageRepository _packageRepository;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageManager _packageManager;
        private readonly IPackageQueryService _packageQueryService;
        #endregion

        #region Constructors
        public ExtensionsViewModel(IPackageQueryService packageQueryService, IDispatcherService dispatcherService,
            IPackageManager packageManager)
        {
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => packageManager);

            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;
            _packageManager = packageManager;

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
        private void OnIsPrereleaseAllowedChanged()
        {
            var updateRepository = _packageRepository as UpdateRepository;
            if (updateRepository != null)
            {
                updateRepository.AllowPrerelease = IsPrereleaseAllowed;
            }

            UpdateRepository();
        }

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
                TotalPackagesCount = _packageRepository.CountPackages(SearchFilter, IsPrereleaseAllowed);
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
                    var packageDetails = _packageQueryService.GetPackages(_packageRepository, IsPrereleaseAllowed, SearchFilter, PackagesToSkip).ToArray();
                    AvailablePackages.Clear();
                    AvailablePackages.AddRange(packageDetails);
                });
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
                versionsOfPackage = _packageQueryService.GetVersionsOfPackage(_packageRepository, SelectedPackage.Package, IsPrereleaseAllowed, ref skip, take);
                accumList.AddRange(versionsOfPackage);
            } while (versionsOfPackage.Any());*/

            UpdatePackages();
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