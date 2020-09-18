namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NuGet.Credentials;
    using Orc.NuGetExplorer.Enums;

    public interface ICredentialProviderLoaderService
    {
        Task<IEnumerable<ICredentialProvider>> GetCredentialProvidersAsync();
        void SetCredentialPolicy(CredentialStoragePolicy storagePolicy);
    }
}
