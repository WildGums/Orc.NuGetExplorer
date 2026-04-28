namespace Orc.NuGetExplorer;

public enum CredentialStoragePolicy
{
    None = 0,
    WindowsVault = 1,
    WindowsVaultConfigurationFallback = 2,
    Configuration = 3
}
