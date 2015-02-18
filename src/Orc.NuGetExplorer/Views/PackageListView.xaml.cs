// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageListView.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using Catel.MVVM.Views;

    public partial class PackageListView
    {
        #region Fields
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof (ObservableCollection<PackageDetails>), typeof (PackageListView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SelectedPackageProperty = DependencyProperty.Register("SelectedPackage",
            typeof (PackageDetails), typeof (PackageListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageListView"/> class.
        /// </summary>
        public PackageListView()
        {
            InitializeComponent();

            ItemsSource = new ObservableCollection<PackageDetails>();
        }

        /// <summary>
        /// Initializes static members of the <see cref="PackageListView"/> class.
        /// </summary>
        /// <remarks>This method is required for design time support.</remarks>
        static PackageListView()
        {
            typeof (PackageListView).AutoDetectViewPropertiesToSubscribe();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public ObservableCollection<PackageDetails> ItemsSource
        {
            get { return (ObservableCollection<PackageDetails>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public PackageDetails SelectedPackage
        {
            get { return (PackageDetails) GetValue(SelectedPackageProperty); }
            set { SetValue(SelectedPackageProperty, value); }
        }
        #endregion
    }
}