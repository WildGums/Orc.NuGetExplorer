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

        /// <summary>
        /// Identifies the <see cref="Collection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
                  DependencyProperty.Register(nameof(Collection), typeof(object), typeof(DependenciesView), new PropertyMetadata(null));
    }
}
