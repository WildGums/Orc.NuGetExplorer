// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineExtensionsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.ObjectModel;
    using Catel;
    using Catel.Collections;
    using Catel.MVVM;
    using Catel.Services;

    public class OnlineExtensionsViewModel : ViewModelBase
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
        }
        #endregion

        #region Properties
        public string PackageSource { get; set; }
        public string SearchFilter { get; set; }
        public ObservableCollection<PackageDetails> AvailablePackages { get; private set; }
        public PackageDetails SelectedPackage { get; set; }
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
            if (!string.IsNullOrWhiteSpace(PackageSource))
            {
                _dispatcherService.BeginInvoke(() => { AvailablePackages.ReplaceRange(_packageQueryService.GetPackages(PackageSource, SearchFilter)); });
            }
        }
        #endregion
    }
}