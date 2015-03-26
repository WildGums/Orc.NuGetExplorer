// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Native;

    internal class AuthenticationProvider : IAuthenticationProvider
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IAuthenticationSilencerService _authenticationSilencerService;
        private readonly IServiceLocator _serviceLocator;
        private IPleaseWaitInterruptService _pleaseWaitInterruptService;
        #endregion

        #region Constructors
        public AuthenticationProvider(IAuthenticationSilencerService authenticationSilencerService, IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => authenticationSilencerService);
            Argument.IsNotNull(() => serviceLocator);

            _authenticationSilencerService = authenticationSilencerService;
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
        public AuthenticationCredentials GetCredentials(Uri uri, bool previousCredentialsFailed)
        {
            Log.Debug("Requesting credentials for '{0}'", uri);

            bool? result = null;

            var credentials = new AuthenticationCredentials(uri);



            using (PleaseWaitInterruptService.InterruptTemporarily())
            {
                DispatchHelper.DispatchIfNecessary(() =>
                {
                    var uriString = uri.ToString().ToLower();

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
                        IsAuthenticationRequired = _authenticationSilencerService.IsAuthenticationRequired
                    };

                    result = credentialsPrompter.ShowDialog();
                    if (result ?? false)
                    {
                        credentials.UserName = credentialsPrompter.UserName;
                        credentials.Password = credentialsPrompter.Password;
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