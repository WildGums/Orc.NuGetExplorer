namespace Orc.NuGetExplorer
{
    using Catel;
    using Catel.Configuration;

    public static class IConfigurationServiceExtensions
    {
        public static CredentialStoragePolicy GetCredentialStoragePolicy(this IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);
            return configurationService.GetRoamingValue(Settings.NuGet.CredentialStorage, CredentialStoragePolicy.WindowsVaultConfigurationFallback);
        }

        public static void SetCredentialStoragePolicy(this IConfigurationService configurationService, CredentialStoragePolicy value)
        {
            Argument.IsNotNull(() => configurationService);
            configurationService.SetRoamingValue(Settings.NuGet.CredentialStorage, value);
        }
    }
}
