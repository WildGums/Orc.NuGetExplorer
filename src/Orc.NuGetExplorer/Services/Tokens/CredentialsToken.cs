namespace Orc.NuGetExplorer;

using System;
using System.Threading.Tasks;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

internal sealed class CredentialsToken : IDisposable
{
    private readonly HttpHandlerResourceV3 _repositoryHttpHandler;

    public static async Task<CredentialsToken?> CreateAsync(SourceRepository repository)
    {
        var resource = await repository.GetResourceAsync<HttpHandlerResourceV3>();
        if (resource is null)
        {
            return null;
        }

        return new CredentialsToken(resource);
    }

    public CredentialsToken(HttpHandlerResourceV3 httpHandler)
    {
        _repositoryHttpHandler = httpHandler;
    }

    public void Dispose()
    {
        _repositoryHttpHandler.ResetCredentials();
    }
}
