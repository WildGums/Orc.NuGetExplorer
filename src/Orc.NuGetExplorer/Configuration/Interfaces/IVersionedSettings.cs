namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using NuGet.Configuration;

    public interface IVersionedSettings : ISettings
    {
        bool IsLastVersion { get; }

        Version? Version { get; }

        Version? MinimalVersion { get; }

        event EventHandler SettingsRead;

        void UpdateMinimalVersion();
        void UpdateVersion();
    }
}
