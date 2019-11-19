namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using Catel.MVVM.Views;
    using Models;
    using Orc.NuGetExplorer.Controls.Helpers;

    /// <summary>
    /// Interaction logic for ExplorerPageView.xaml
    /// </summary>
    internal partial class ExplorerPageView : Catel.Windows.Controls.UserControl
    {
        private const string ArrowUpResourceKey = "ArrowUpBadgeContent";
        private const string ArrowDownResourceKey = "ArrowDownBadgeContent";
        private const int IndicatorOffset = 2;

        private readonly FrameworkElement _arrowUpResource;
        private readonly FrameworkElement _arrowDownResource;

        private ScrollViewer _infinityboxScrollViewer;

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

        private void Border_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            //fix loading indicator part size
            _infinityboxScrollViewer = _infinityboxScrollViewer ?? WpfHelper.FindVisualChild<ScrollViewer>(infinitybox);

            if (_infinityboxScrollViewer == null)
            {
                return;
            }

            if (indicatorScreen.Visibility == Visibility.Visible)
            {
                //try to resize indicator border to viewport width
                indicatorScreen.SetCurrentValue(WidthProperty, _infinityboxScrollViewer.ViewportWidth + IndicatorOffset);
            }
        }
    }
}
