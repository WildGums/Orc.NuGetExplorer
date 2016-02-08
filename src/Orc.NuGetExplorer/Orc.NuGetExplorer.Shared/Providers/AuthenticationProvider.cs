// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Scoping;
    using Native;
    using Scopes;

    internal class AuthenticationProvider : IAuthenticationProvider
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        
        private readonly IConfigurationService _configurationService;

        // Ideally we would inject this, but some classes are constructed in ModuleInitializer which don't allow
        // any other assemblies to implement their own version
        private readonly Lazy<IPleaseWaitInterruptService> _pleaseWaitInterruptService = new Lazy<IPleaseWaitInterruptService>(() =>
        {
            var serviceLocator = ServiceLocator.Default;
            return serviceLocator.ResolveType<IPleaseWaitInterruptService>();
        });
        #endregion

        #region Constructors
        public AuthenticationProvider(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;
        }
        #endregion

        #region Methods
        public async Task<AuthenticationCredentials> GetCredentialsAsync(Uri uri, bool previousCredentialsFailed)
        {
            Log.Debug("Requesting credentials for '{0}'", uri);

            bool? result = null;

            var credentials = new AuthenticationCredentials(uri);

            using (_pleaseWaitInterruptService.Value.InterruptTemporarily())
            {
                await DispatchHelper.DispatchIfNecessaryAsync(() =>
                {
                    var uriString = uri.ToString().ToLower();

                    using (var scopeManager = ScopeManager<AuthenticationScope>.GetScopeManager(uriString.GetSafeScopeName(), () => new AuthenticationScope()))
                    {
                        var authenticationScope = scopeManager.ScopeObject;

                        var credentialsPrompter = new CredentialsPrompter(_configurationService)
                        {
                            Target = uriString,
                            UserName = string.Empty,
                            Password = string.Empty,
                            AllowStoredCredentials = !previousCredentialsFailed,
                            ShowSaveCheckBox = true,
                            WindowTitle = "Credentials required",
                            MainInstruction = "Credentials are required to access this feed",
                            Content = string.Format("In order to continue, please enter the credentials for {0} below.", uri),
                            IsAuthenticationRequired = authenticationScope.CanPromptForAuthentication
                        };

                        authenticationScope.HasPromptedForAuthentication = true;

                        result = credentialsPrompter.ShowDialog();
                        if (result ?? false)
                        {
                            credentials.UserName = credentialsPrompter.UserName;
                            credentials.Password = credentialsPrompter.Password;
                        }
                        else
                        {
                            credentials.StoreCredentials = false;
                        }
                    }
                });
            }

            if (result ?? false)
            {
                Log.Debug("Successfully requested credentials for '{0}' using user '{1}'", uri, credentials.UserName);

                return credentials;
            }

            Log.Debug("Failed to request credentials for '{0}'", uri);

            return null;
        }
        #endregion
    }
}