namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;
    using Models;

    /// <summary>
    /// Interaction logic for ExplorerPageView.xaml
    /// </summary>
    public partial class ExplorerPageView : Catel.Windows.Controls.UserControl
    {
        private const string ArrowUpResourceKey = "ArrowUpBadgeContent";
        private const string ArrowDownResourceKey = "ArrowDownBadgeContent";

        private readonly FrameworkElement _arrowUpResource;
        private readonly FrameworkElement _arrowDownResource;

        static ExplorerPageView()
        {
            typeof(ExplorerPageView).AutoDetectViewPropertiesToSubscribe();
        }

        public ExplorerPageView()
        {
            //prevent closing view models
            InitializeComponent();

            _arrowUpResource = FindResource(ArrowUpResourceKey) as FrameworkElement;
            _arrowDownResource = FindResource(ArrowDownResourceKey) as FrameworkElement;
        }

        [ViewToViewModel(viewModelPropertyName: "SelectedPackageItem", MappingType = ViewToViewModelMappingType.TwoWayViewModelWins)]
        public NuGetPackage SelectedItemOnPage
        {
            get { return (NuGetPackage)GetValue(SelectedItemOnPageProperty); }
            set { SetValue(SelectedItemOnPageProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemOnPageProperty =
            DependencyProperty.Register(nameof(SelectedItemOnPage), typeof(NuGetPackage), typeof(ExplorerPageView), new PropertyMetadata(null));
    }
}
