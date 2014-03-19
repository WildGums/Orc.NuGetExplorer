namespace Orc.NuGetExplorer.Views
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using Catel.MVVM.Views;
    using Catel.Windows.Controls;
    using NuGet;
    using Orc.NuGetExplorer.Models;

    /// <summary>
    /// Interaction logic for PackageListView.xaml.
    /// </summary>
    public partial class PackageListView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageListView"/> class.
        /// </summary>
        public PackageListView()
        {
            InitializeComponent();

            ItemsSource = new ObservableCollection<PackageDetails>();
        }

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public ObservableCollection<PackageDetails> ItemsSource
        {
            get { return (ObservableCollection<PackageDetails>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof(ObservableCollection<PackageDetails>), typeof(PackageListView), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public PackageDetails SelectedPackage
        {
            get { return (PackageDetails)GetValue(SelectedPackageProperty); }
            set { SetValue(SelectedPackageProperty, value); }
        }

        public static readonly DependencyProperty SelectedPackageProperty = DependencyProperty.Register("SelectedPackage",
            typeof(PackageDetails), typeof(PackageListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
