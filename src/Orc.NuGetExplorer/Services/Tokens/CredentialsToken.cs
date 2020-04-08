namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;

    internal class CredentialsToken : IDisposable
    {
        private readonly HttpHandlerResourceV3 _repositoryHttpHandler;

        public static async Task<CredentialsToken> Create(SourceRepository repository)
        {
            return new CredentialsToken(await repository.GetResourceAsync<HttpHandlerResourceV3>());
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
}
