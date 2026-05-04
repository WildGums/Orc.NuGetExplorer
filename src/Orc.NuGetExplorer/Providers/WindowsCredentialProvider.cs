namespace Orc.NuGetExplorer.Providers;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Catel.Configuration;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using NuGet.Credentials;
using Orc.NuGetExplorer.Windows;

public class WindowsCredentialProvider : ICredentialProvider
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(WindowsCredentialProvider));

    private readonly IConfigurationService _configurationService;
    private readonly bool _canAccessStoredCredentials;

    public WindowsCredentialProvider(IConfigurationService configurationService)
    {
        ArgumentNullException.ThrowIfNull(configurationService);

        _configurationService = configurationService;
        _canAccessStoredCredentials = _configurationService.GetCredentialStoragePolicy() != CredentialStoragePolicy.None;
    }

    public string Id => "Windows Credentials";

    public async Task<CredentialResponse> GetAsync(Uri uri, IWebProxy proxy, CredentialRequestType type, string message, bool isRetry, bool nonInteractive, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (isRetry)
        {
            Logger.LogDebug("Retrying to request credentials for '{Uri}'", uri);
        }
        else
        {
            Logger.LogDebug("Requesting credentials for '{Uri}'", uri);
        }

        var uriString = uri.ToString().ToLower();

        var credentialsPrompter = new CredentialsPrompter(_configurationService, uriString)
        {
            AllowStoredCredentials = !isRetry && _canAccessStoredCredentials,
            ShowSaveCheckBox = true,
            WindowTitle = "Credentials required",
            MainInstruction = "Credentials are required to access this feed",
            Content = message,
            IsAuthenticationRequired = true
        };

        bool? result = credentialsPrompter.ShowDialog();
        if (result ?? false)
        {
            //creating success response

            Logger.LogDebug("Successfully requested credentials for '{0}' using user '{1}'", uri, credentialsPrompter.UserName);

            //creating network credentials
            var nugetCredentials = new NetworkCredential(credentialsPrompter.UserName, credentialsPrompter.Password);

            var response = new CredentialResponse(nugetCredentials);

            return response;
        }
        else
        {
            Logger.LogDebug("Failed to request credentials for '{0}'", uri);
            return new CredentialResponse(CredentialStatus.UserCanceled);
        }
    }
}
