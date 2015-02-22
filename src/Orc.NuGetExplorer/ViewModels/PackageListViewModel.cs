// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageListViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.ObjectModel;
    using Catel.MVVM;

    public class PackageListViewModel : ViewModelBase
    {
        #region Constructors
        public PackageListViewModel()
        {
            
        }
        #endregion

        #region Properties
        public ObservableCollection<PackageDetails> ItemsSource { get; set; }
        public PackageDetails SelectedPackage { get; set; }
        public string ButtonHeader { get; set; }
        #endregion
    }
}