namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ExplorerTopBarView.xaml
    /// </summary>
    internal partial class ExplorerTopBarView : Catel.Windows.Controls.UserControl
    {
        public ExplorerTopBarView()
        {
            InitializeComponent();
        }

        public TabControl UsedOn
        {
            get { return (TabControl)GetValue(UsedOnProperty); }
            set { SetValue(UsedOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsedOnProperty =
            DependencyProperty.Register("UsedOn", typeof(TabControl), typeof(ExplorerTopBarView), new PropertyMetadata(null));
    }
}
