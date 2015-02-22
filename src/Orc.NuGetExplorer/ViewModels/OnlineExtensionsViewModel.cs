// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineExtensionsViewModel.cs" company="Wild Gums">
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

    internal class OnlineExtensionsViewModel : ViewModelBase
    {
        #region Fields
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageQueryService _packageQueryService;
        #endregion

        #region Constructors
        public OnlineExtensionsViewModel(IPackageQueryService packageQueryService, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);

            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;

            AvailablePackages = new ObservableCollection<PackageDetails>();

            PackageAction = new Command(OnPackageActionExecute);

            Search();
        }
        #endregion

        #region Properties
        public string SearchFilter { get; set; }
        public PackageDetails SelectedPackage { get; set; }
        public PackageSourcesNavigationItem PackageSource { get; set; }
        public ObservableCollection<PackageDetails> AvailablePackages { get; private set; }
        public int TotalPackagesCount { get; set; }
        #endregion

        #region Methods
        private void OnPackageSourceChanged()
        {
            Search();
        }

        private void OnSearchFilterChanged()
        {
            Search();
        }

        private void Search()
        {
            if (PackageSource != null)
            {
                _dispatcherService.BeginInvoke(() =>
                {
                    var packageSources = PackageSource.PackageSources;
                    var packageDetails = _packageQueryService.GetPackages(packageSources, SearchFilter).ToArray();
                    AvailablePackages.ReplaceRange(packageDetails);
                   // TotalPackagesCount = _packageQueryService.
                });
            }
        }
        #endregion

        #region Commands
        public Command PackageAction { get; private set; }

        private void OnPackageActionExecute()
        {
        }
        #endregion
    }
}