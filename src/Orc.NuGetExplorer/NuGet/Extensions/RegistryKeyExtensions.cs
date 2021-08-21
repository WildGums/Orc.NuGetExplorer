namespace Orc.NuGetExplorer
{
    using Microsoft.Win32;

    public static class RegistryKeyExtensions
    {
        public static bool TryGetInstallFlag(this RegistryKey versionRegistryKey, out string installFlagValue)
        {
            // Get the installation flag, or an empty string if there is none.
            installFlagValue = versionRegistryKey.GetValue("Install", "").ToString();

            return !string.IsNullOrEmpty(installFlagValue);
        }
    }
}
