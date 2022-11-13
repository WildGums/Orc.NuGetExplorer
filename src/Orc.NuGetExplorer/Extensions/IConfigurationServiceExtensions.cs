namespace Orc.NuGetExplorer
{
    using System;
    using Catel.Configuration;

    public static class IConfigurationServiceExtensions
    {
        public static CredentialStoragePolicy GetCredentialStoragePolicy(this IConfigurationService configurationService)
        {
            ArgumentNullException.ThrowIfNull(configurationService);

            return configurationService.GetRoamingValue(Settings.NuGet.CredentialStorage, CredentialStoragePolicy.WindowsVaultConfigurationFallback);
        }

        public static void SetCredentialStoragePolicy(this IConfigurationService configurationService, CredentialStoragePolicy value)
        {
            ArgumentNullException.ThrowIfNull(configurationService);

            configurationService.SetRoamingValue(Settings.NuGet.CredentialStorage, value);
        }
    }
}
