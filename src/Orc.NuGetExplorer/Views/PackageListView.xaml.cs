// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageListView.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;
    using Catel.MVVM.Views;

    internal partial class PackageListView
    {
        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="PackageListView"/> class.
        /// </summary>
        /// <remarks>This method is required for design time support.</remarks>
        static PackageListView()
        {
            typeof (PackageListView).AutoDetectViewPropertiesToSubscribe();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageListView"/> class.
        /// </summary>
        public PackageListView()
        {
            InitializeComponent();

            ItemsSource = new ObservableCollection<PackageDetails>();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public ObservableCollection<PackageDetails> ItemsSource
        {
            get { return (ObservableCollection<PackageDetails>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<PackageDetails>),
            typeof(PackageListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));        

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewModelWins)]
        public IPackageDetails SelectedPackage
        {
            get { return (IPackageDetails) GetValue(SelectedPackageProperty); }
            set { SetValue(SelectedPackageProperty, value); }
        }

        public static readonly DependencyProperty SelectedPackageProperty = DependencyProperty.Register("SelectedPackage", typeof(IPackageDetails),
            typeof(PackageListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        

        public string ButtonHeader
        {
            get { return (string) GetValue(ButtonHeaderProperty); }
            set { SetValue(ButtonHeaderProperty, value); }
        }

        public static readonly DependencyProperty ButtonHeaderProperty = DependencyProperty.Register("ButtonHeader",
            typeof(string), typeof(PackageListView), new FrameworkPropertyMetadata(null));        

        public ICommand PackageCommand
        {
            get { return (ICommand) GetValue(PackageCommandProperty); }
            set { SetValue(PackageCommandProperty, value); }
        }

        public static readonly DependencyProperty PackageCommandProperty = DependencyProperty.Register("PackageCommand",
            typeof(ICommand), typeof(PackageListView), new UIPropertyMetadata(null));
        #endregion
    }
}