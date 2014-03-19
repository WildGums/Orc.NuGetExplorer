namespace Orc.NuGetExplorer.Views
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using Catel.MVVM.Views;
    using Catel.Windows.Controls;
    using NuGet;

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

            ItemsSource = new ObservableCollection<IPackageMetadata>();
        }

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public ObservableCollection<IPackageMetadata> ItemsSource
        {
            get { return (ObservableCollection<IPackageMetadata>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof(ObservableCollection<IPackageMetadata>), typeof(PackageListView), new PropertyMetadata(null));


        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public IPackageMetadata SelectedPackage
        {
            get { return (IPackageMetadata)GetValue(SelectedPackageProperty); }
            set { SetValue(SelectedPackageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPackage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPackageProperty = DependencyProperty.Register("SelectedPackage",
            typeof(IPackageMetadata), typeof(PackageListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
