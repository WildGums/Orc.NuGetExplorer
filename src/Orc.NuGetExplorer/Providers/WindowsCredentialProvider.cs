namespace Orc.NuGetExplorer.Providers
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;
    using NuGet.Configuration;
    using NuGet.Credentials;
    using NuGetExplorer.Native;

    public class WindowsCredentialProvider : ICredentialProvider
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;
        private readonly bool _canAccessStoredCredentials;

        public WindowsCredentialProvider(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;
            _canAccessStoredCredentials = _configurationService.GetCredentialStoragePolicy() != CredentialStoragePolicy.None;
        }

        public string Id => "Windows Credentials";

        public async Task<CredentialResponse> GetAsync(Uri uri, IWebProxy proxy, CredentialRequestType type, string message, bool isRetry, bool nonInteractive, CancellationToken cancellationToken)
        {
            if (isRetry)
            {
                Log.Debug($"Retrying to request credentials for '{uri}'");
            }
            else
            {
                Log.Debug($"Requesting credentials for '{uri}'");
            }

            bool? result = null;

            var uriString = uri.ToString().ToLower();

            var credentialsPrompter = new CredentialsPrompter(_configurationService)
            {
                Target = uriString,
                AllowStoredCredentials = !isRetry && _canAccessStoredCredentials,
                ShowSaveCheckBox = true,
                WindowTitle = "Credentials required",
                MainInstruction = "Credentials are required to access this feed",
                Content = message,
                IsAuthenticationRequired = true
            };

            result = credentialsPrompter.ShowDialog();

            if (result ?? false)
            {
                //creating success response

                Log.Debug("Successfully requested credentials for '{0}' using user '{1}'", uri, credentialsPrompter.UserName);

                //creating network credentials
                var nugetCredentials = new NetworkCredential(credentialsPrompter.UserName, credentialsPrompter.Password);

                var response = new CredentialResponse(nugetCredentials);

                return response;
            }
            else
            {
                Log.Debug("Failed to request credentials for '{0}'", uri);
                return new CredentialResponse(CredentialStatus.UserCanceled);
            }
        }
    }
}
