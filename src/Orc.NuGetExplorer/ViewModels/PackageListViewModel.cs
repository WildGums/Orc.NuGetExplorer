// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageListViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.ObjectModel;
    using Catel.MVVM;
    using Orc.NuGetExplorer.Models;

    public class PackageListViewModel : ViewModelBase
    {
        public PackageListViewModel()
        {
        }

        public ObservableCollection<PackageDetails> ItemsSource { get; set; }

        public PackageDetails SelectedPackage { get; set; }
    }
}