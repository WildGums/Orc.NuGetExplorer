namespace Orc.NuGetExplorer.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ExplorerWindow.xaml
    /// </summary>
    public partial class ExplorerWindow : Catel.Windows.DataWindow
    {
        public ExplorerWindow()
        {
            InitializeComponent();
            ShowInTaskbar = true;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}
