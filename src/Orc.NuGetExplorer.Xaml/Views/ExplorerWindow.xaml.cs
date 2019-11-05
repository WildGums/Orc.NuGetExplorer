namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.Windows;
    using Orc.NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for ExplorerWindow.xaml
    /// </summary>
    internal partial class ExplorerWindow : DataWindow
    {
        public ExplorerWindow() : this(null)
        {
        }

        public ExplorerWindow(ExplorerViewModel viewModel) : base(viewModel, DataWindowMode.Custom)
        {
            InitializeComponent();
            ShowInTaskbar = true;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Title = viewModel.Title;
        }
    }
}
