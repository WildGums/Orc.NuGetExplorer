// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageListViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.ObjectModel;
    using Catel.MVVM;

    internal class PackageListViewModel : ViewModelBase
    {
        #region Constructors
        public PackageListViewModel()
        {
        }
        #endregion

        #region Properties
        public ObservableCollection<IPackageDetails> ItemsSource { get; set; }
        public IPackageDetails SelectedPackage { get; set; }
        #endregion
    }
}