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
    using NuGet;

    internal class OnlineExtensionsViewModel : ViewModelBase
    {
        #region Fields
        private IPackageRepository _packageRepository;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IPackageRepositoryService _packageRepositoryService;
        #endregion

        #region Constructors
        public OnlineExtensionsViewModel(IPackageRepositoryService packageRepositoryService, IPackageQueryService packageQueryService, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => packageRepositoryService);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);

            _packageRepositoryService = packageRepositoryService;
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
        public int PackagesToSkip { get; set; }
        public string ActionName { get; set; }
        #endregion

        #region Methods
        private void OnPackagesToSkipChanged()
        {
            Search();
        }
        
        private void OnPackageSourceChanged()
        {
            UpdateRepository();
            Search();
        }

        private void OnSearchFilterChanged()
        {
            Search();
        }

        private void OnActionNameChanged()
        {
            UpdateRepository();
        }

        private void UpdateRepository()
        {
            var packageSources = PackageSource.PackageSources;
            _packageRepository = _packageRepositoryService.GetRepository(ActionName, packageSources);
            TotalPackagesCount = _packageQueryService.GetPackagesCount(_packageRepository, SearchFilter);
        }

        private void Search()
        {
            if (PackageSource != null)
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
        }
        #endregion
    }
}