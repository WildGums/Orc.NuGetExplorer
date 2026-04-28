namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.Credentials;

public interface ICredentialProviderLoaderService
{
    Task<IReadOnlyList<ICredentialProvider>> GetCredentialProvidersAsync();
    void SetCredentialPolicy(CredentialStoragePolicy storagePolicy);
}
