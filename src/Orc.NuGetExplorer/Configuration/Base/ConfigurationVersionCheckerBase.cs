namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using Catel;
    using NuGet.Configuration;

    public abstract class ConfigurationVersionCheckerBase
    {
        private readonly IVersionedSettings _settings;

        protected ConfigurationVersionCheckerBase(ISettings settings)
        {
            Argument.IsNotNull(() => settings);
            Argument.IsOfType(() => settings, typeof(IVersionedSettings));

            _settings = settings as IVersionedSettings;

        }

        public event EventHandler Updated;

        public virtual void Check()
        {
            if (_settings.IsLastVersion)
            {
                return;
            }
        }

        protected void RaiseUpdated(EventArgs e)
        {
            Updated?.Invoke(this, e);
        }
    }
}
