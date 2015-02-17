// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineExtensionsViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
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
        private readonly IPackageQueryService _packageQueryService;
        private readonly IDispatcherService _dispatcherService;

        public OnlineExtensionsViewModel(IPackageQueryService packageQueryService, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);

            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;

            AvailablePackages = new ObservableCollection<PackageDetails>();
        }

        public string PackageSource { get; set; }

        private void OnPackageSourceChanged()
        {
            Search();
        }

        public string SearchFilter { get; set; }

        public ObservableCollection<PackageDetails> AvailablePackages { get; private set; } 

        public PackageDetails SelectedPackage { get; set; }

        private void OnSearchFilterChanged()
        {
            Search();
        }

        private void Search()
        {
            if (!string.IsNullOrWhiteSpace(PackageSource))
            {
                _dispatcherService.BeginInvoke(() =>
                {
                    AvailablePackages.ReplaceRange(_packageQueryService.GetPackages(PackageSource, SearchFilter));
                });
            }
        }
    }
}