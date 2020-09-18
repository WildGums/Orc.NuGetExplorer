namespace Orc.NuGetExplorer.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum CredentialStoragePolicy
    {
        None = 0,
        WindowsVault,
        WindowsVaultConfigurationFallback,
        Configuration
    }
}
