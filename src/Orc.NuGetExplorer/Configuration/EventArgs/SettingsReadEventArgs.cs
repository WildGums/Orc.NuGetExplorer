namespace Orc.NuGetExplorer.Configuration
{
    using System;

    public class SettingsReadEventArgs : EventArgs
    {
        public SettingsReadEventArgs(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}
