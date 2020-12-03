namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NuGet.Credentials;

    public interface ICredentialProviderLoaderService
    {
        Task<IEnumerable<ICredentialProvider>> GetCredentialProvidersAsync();
        void SetCredentialPolicy(CredentialStoragePolicy storagePolicy);
    }
}
