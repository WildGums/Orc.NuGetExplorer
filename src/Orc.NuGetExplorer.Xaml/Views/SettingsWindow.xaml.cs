namespace Orc.NuGetExplorer.Views
{
    using Catel.Windows;
    using Orc.NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    internal partial class SettingsWindow : DataWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public SettingsWindow(SettingsViewModel viewModel) : base(viewModel, DataWindowMode.OkCancel)
        {
            InitializeComponent();

            Title = viewModel.Title;
        }
    }
}
