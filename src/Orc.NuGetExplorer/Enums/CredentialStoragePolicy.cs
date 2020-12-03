namespace Orc.NuGetExplorer
{
    public enum CredentialStoragePolicy
    {
        None = 0,
        WindowsVault,
        WindowsVaultConfigurationFallback,
        Configuration
    }
}
