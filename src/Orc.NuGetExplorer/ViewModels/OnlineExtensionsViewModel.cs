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
    using NuGet;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Services;

    public class OnlineExtensionsViewModel : ViewModelBase
    {
        private readonly IPackageQueryService _packageQueryService;

        public OnlineExtensionsViewModel(IPackageQueryService packageQueryService)
        {
            Argument.IsNotNull(() => packageQueryService);

            _packageQueryService = packageQueryService;

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
                AvailablePackages.ReplaceRange(_packageQueryService.GetPackages(PackageSource, SearchFilter));
            }
        }
    }
}