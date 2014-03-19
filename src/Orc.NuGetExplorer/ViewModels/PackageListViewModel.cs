// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageListViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.ObjectModel;
    using Catel;
    using Catel.MVVM;
    using NuGet;

    public class PackageListViewModel : ViewModelBase
    {
        public PackageListViewModel(IPackageMetadata selectedPackage)
        {
            Argument.IsNotNull(() => selectedPackage);

            SelectedPackage = selectedPackage;
        }

        public ObservableCollection<IPackageMetadata> ItemsSource { get; set; }

        public IPackageMetadata SelectedPackage { get; private set; }
    }
}