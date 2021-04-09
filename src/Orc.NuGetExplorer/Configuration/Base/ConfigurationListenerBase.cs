namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Configuration;

    public abstract class ConfigurationListenerBase
    {
        private readonly IVersionedSettings _settings;

        protected ConfigurationListenerBase(ISettings settings)
        {
            Argument.IsNotNull(() => settings);
            Argument.IsOfType(() => settings, typeof(IVersionedSettings));

            _settings = settings as IVersionedSettings;

            _settings.SettingsRead += OnConfigurationSettingsRead;
        }

        public async virtual Task OnSettingsReadAsync()
        {

        }

        private async void OnConfigurationSettingsRead(object sender, EventArgs e)
        {
            await OnSettingsReadAsync();
        }
    }
}
