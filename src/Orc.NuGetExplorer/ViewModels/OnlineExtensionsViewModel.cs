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
    using Catel.Fody;
    using Catel.MVVM;
    using Catel.Services;
    using NuGet;

    internal class OnlineExtensionsViewModel : ViewModelBase
    {
        [Model]
        [Expose("PackageSource")]
        public NavigationItemsGroup NavigationItemsGroup { get; private set; }

        #region Fields
        private readonly IDispatcherService _dispatcherService;
        private readonly IPackageQueryService _packageQueryService;
        #endregion

        #region Constructors
        public OnlineExtensionsViewModel(NavigationItemsGroup navigationItemsGroup, IPackageQueryService packageQueryService, IDispatcherService dispatcherService)
        {
            NavigationItemsGroup = navigationItemsGroup;
            Argument.IsNotNull(() => navigationItemsGroup);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => dispatcherService);

            _packageQueryService = packageQueryService;
            _dispatcherService = dispatcherService;

            AvailablePackages = new ObservableCollection<PackageDetails>();
        }
        #endregion

        #region Properties
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
            if (NavigationItemsGroup.PackageSource != null)
            {
                _dispatcherService.BeginInvoke(() => { AvailablePackages.ReplaceRange(_packageQueryService.GetPackages(NavigationItemsGroup.PackageSource.Name, SearchFilter)); });
            }
        }
        #endregion
    }
}