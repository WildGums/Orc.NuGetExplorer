namespace Orc.NuGetExplorer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Catel.Windows.DataWindow
    {
        public MainWindow() : base(Catel.Windows.DataWindowMode.Custom)
        {
            InitializeComponent();
            ShowInTaskbar = true;

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
    }
}
