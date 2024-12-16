namespace Orc.NuGetExplorer.Views;

using System;
using Catel.IoC;
using Catel.Services;
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
        Title = viewModel.Title;

        InitializeComponent();
    }

    protected override void OnLoaded(EventArgs e)
    {
        base.OnLoaded(e);

        var appDataService = ServiceLocator.Default.ResolveType<IAppDataService>();
        appDataService?.LoadWindowSize(this, true);
    }

    protected override void OnUnloaded(EventArgs e)
    {
        var appDataService = ServiceLocator.Default.ResolveType<IAppDataService>();
        appDataService?.SaveWindowSize(this);

        base.OnUnloaded(e);
    }
}
