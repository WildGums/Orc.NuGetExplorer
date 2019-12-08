namespace Orc.NuGetExplorer.Views
{
    using System;
    using Catel.Windows;
    using Orc.Controls;
    using Orc.NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    internal partial class NuGetSettingsWindow : DataWindow
    {
        public NuGetSettingsWindow()
        {
            InitializeComponent();
        }

        public NuGetSettingsWindow(NuGetSettingsViewModel viewModel) 
            : base(viewModel, DataWindowMode.OkCancel)
        {
            if(viewModel.CanReset)
            {
                AddCustomButton(new DataWindowButton("Reset", "Reset"));
            }

            Title = viewModel.Title;

            InitializeComponent();
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
