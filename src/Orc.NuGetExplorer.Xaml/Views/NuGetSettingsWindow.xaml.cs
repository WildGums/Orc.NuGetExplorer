namespace Orc.NuGetExplorer.Views;

using System;
using Catel.IoC;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Orc.Controls;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
internal partial class NuGetSettingsWindow
{
    protected override void OnLoaded(EventArgs e)
    {
        base.OnLoaded(e);

        var appDataService = IoCContainer.ServiceProvider.GetService<IAppDataService>();
        appDataService?.LoadWindowSize(this, true);
    }

    protected override void OnUnloaded(EventArgs e)
    {
        var appDataService = IoCContainer.ServiceProvider.GetService<IAppDataService>();
        appDataService?.SaveWindowSize(this);

        base.OnUnloaded(e);
    }
}
