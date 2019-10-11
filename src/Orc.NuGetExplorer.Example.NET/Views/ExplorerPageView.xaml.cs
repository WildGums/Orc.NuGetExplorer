using Catel.MVVM;
using Catel.MVVM.Views;
using System.Windows;

namespace Orc.NuGetExplorer.Views
{
    /// <summary>
    /// Interaction logic for ExplorerPageView.xaml
    /// </summary>
    public partial class ExplorerPageView : Catel.Windows.Controls.UserControl
    {
        const string ArrowUpResourceKey = "ArrowUpBadgeContent";
        const string ArrowDownResourceKey = "ArrowDownBadgeContent";

        FrameworkElement ArrowUpResource;
        FrameworkElement ArrowDownResource;

        public ExplorerPageView()
        {
            //prevent closing view models
            InitializeComponent();

            ArrowUpResource = FindResource(ArrowUpResourceKey) as FrameworkElement;
            ArrowDownResource = FindResource(ArrowDownResourceKey) as FrameworkElement;
        }

        [ViewToViewModel(viewModelPropertyName: "SelectedPackageItem", MappingType = ViewToViewModelMappingType.TwoWayViewModelWins)]
        public IViewModel SelectedItemOnPage
        {
            get { return (IViewModel)GetValue(SelectedItemOnPageProperty); }
            set { SetValue(SelectedItemOnPageProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemOnPageProperty =
            DependencyProperty.Register(nameof(SelectedItemOnPage), typeof(IViewModel), typeof(ExplorerPageView), new PropertyMetadata(null));
    }
}
