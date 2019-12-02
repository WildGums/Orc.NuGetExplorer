namespace Orc.NuGetExplorer.Views
{
    using System;
    using Catel.Windows;
    using Orc.Controls;
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

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            this.LoadWindowSize(true);
        }

        protected override void OnUnloaded(EventArgs e)
        {
            this.SaveWindowSize();

            base.OnUnloaded(e);
        }
    }
}
