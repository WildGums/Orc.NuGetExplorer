namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.Windows;

    /// <summary>
    /// Interaction logic for ExplorerWindow.xaml
    /// </summary>
    internal partial class ExplorerWindow : DataWindow
    {
        public ExplorerWindow()
        {
            InitializeComponent();
            ShowInTaskbar = true;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}
