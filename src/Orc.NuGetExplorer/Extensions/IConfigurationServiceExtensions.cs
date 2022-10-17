namespace Orc.NuGetExplorer
{
    using Catel.Configuration;

    public static class IConfigurationServiceExtensions
    {
        public static CredentialStoragePolicy GetCredentialStoragePolicy(this IConfigurationService configurationService)
        {
            return configurationService.GetRoamingValue(Settings.NuGet.CredentialStorage, CredentialStoragePolicy.WindowsVaultConfigurationFallback);
        }

        public static void SetCredentialStoragePolicy(this IConfigurationService configurationService, CredentialStoragePolicy value)
        {
            configurationService.SetRoamingValue(Settings.NuGet.CredentialStorage, value);
        }
    }
}
