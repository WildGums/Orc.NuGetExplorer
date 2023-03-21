namespace Orc.NuGetExplorer.Configuration;

using System;
using System.Threading.Tasks;
using NuGet.Configuration;

public abstract class ConfigurationListenerBase
{
    private readonly IVersionedSettings _settings;

    protected ConfigurationListenerBase(ISettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _settings = settings as IVersionedSettings ?? throw new InvalidOperationException($"'{nameof(settings)}' must have defined Version");

        _settings.SettingsRead += OnConfigurationSettingsRead;
    }

    public virtual async Task OnSettingsReadAsync()
    {

    }

    private async void OnConfigurationSettingsRead(object? sender, EventArgs e)
    {
        await OnSettingsReadAsync();
    }
}
