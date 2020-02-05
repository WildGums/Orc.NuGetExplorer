namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;
    using Catel.Windows.Controls;

    /// <summary>
    /// Interaction logic for DependenciesView.xaml
    /// </summary>
    internal partial class DependenciesView : UserControl
    {
        static DependenciesView()
        {
            typeof(DependenciesView).AutoDetectViewPropertiesToSubscribe();
        }

        public DependenciesView()
        {
            InitializeComponent();
        }

        [ViewToViewModel("Collection", MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public object Collection
        {
            get { return GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(object), typeof(DependenciesView), new PropertyMetadata(null));
    }
}
