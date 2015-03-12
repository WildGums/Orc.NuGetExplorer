// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Logging;
    using Catel.Services;
    using Native;

    internal class AuthenticationProvider : IAuthenticationProvider
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IDispatcherService _dispatcherService;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public AuthenticationProvider(IDispatcherService dispatcherService, IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => pleaseWaitService);

            _dispatcherService = dispatcherService;
            _pleaseWaitService = pleaseWaitService;
        }
        #endregion

        #region Methods
        public AuthenticationCredentials GetCredentials(Uri uri, bool previousCredentialsFailed)
        {
            Log.Debug("Requesting credentials for '{0}'", uri);

            bool? result = null;

            var credentials = new AuthenticationCredentials(uri);

            using (_pleaseWaitService.HideTemporarily())
            {
                _dispatcherService.Invoke(() =>
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
                        Content = string.Format("In order to continue, please enter the credentials for {0} below.", uri)
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