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
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Scoping;
    using Native;
    using Scopes;

    internal class AuthenticationProvider : IAuthenticationProvider
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IServiceLocator _serviceLocator;
        private IPleaseWaitInterruptService _pleaseWaitInterruptService;
        #endregion

        #region Constructors
        public AuthenticationProvider(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            _serviceLocator = serviceLocator;
        }
        #endregion

        private IPleaseWaitInterruptService PleaseWaitInterruptService
        {
            get
            {
                if (_pleaseWaitInterruptService == null)
                {
                    _pleaseWaitInterruptService = _serviceLocator.ResolveType<IPleaseWaitInterruptService>();
                }

                return _pleaseWaitInterruptService;
            }
        }

        #region Methods
        public async Task<AuthenticationCredentials> GetCredentialsAsync(Uri uri, bool previousCredentialsFailed)
        {
            Log.Debug("Requesting credentials for '{0}'", uri);

            bool? result = null;

            var credentials = new AuthenticationCredentials(uri);

            using (PleaseWaitInterruptService.InterruptTemporarily())
            {
                await DispatchHelper.DispatchIfNecessaryAsync(() =>
                {
                    var uriString = uri.ToString().ToLower();

                    using (var scopeManager = ScopeManager<AuthenticationScope>.GetScopeManager(uriString.GetSafeScopeName(), () => new AuthenticationScope()))
                    {
                        var authenticationScope = scopeManager.ScopeObject;

                        var credentialsPrompter = new CredentialsPrompter
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