using System.Windows;

namespace Orc.NuGetExplorer.Views
{
    /// <summary>
    /// Interaction logic for ExplorerTopBarView.xaml
    /// </summary>
    public partial class ExplorerTopBarView : Catel.Windows.Controls.UserControl
    {
        public ExplorerTopBarView()
        {
            InitializeComponent();
        }

        public DependencyObject UsedOn
        {
            get { return (DependencyObject)GetValue(UsedOnProperty); }
            set { SetValue(UsedOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsedOnProperty =
            DependencyProperty.Register("UsedOn", typeof(DependencyObject), typeof(ExplorerTopBarView), new PropertyMetadata(null));
    }
}
