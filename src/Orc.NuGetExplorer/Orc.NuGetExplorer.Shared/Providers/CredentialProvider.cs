// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Catel;
    using Catel.Scoping;
    using NuGet;
    using Scopes;

    internal class CredentialProvider : ICredentialProvider
    {
        #region Fields
        private readonly IAuthenticationProvider _authenticationProvider;
        #endregion

        #region Constructors
        public CredentialProvider(IAuthenticationProvider authenticationProvider)
        {
            Argument.IsNotNull(() => authenticationProvider);

            _authenticationProvider = authenticationProvider;
        }
        #endregion

        #region Methods
        public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
        {
            // Note: this might cause deadlock, but NuGet is sync while we need async, so keep it this way
            var credentialsTask = _authenticationProvider.GetCredentialsAsync(uri, retrying);
            var credentials = credentialsTask.Result;

            if (credentials == null || (string.IsNullOrWhiteSpace(credentials.UserName) && string.IsNullOrWhiteSpace(credentials.Password)))
            {
                return null;
            }

            return new NetworkCredential(credentials.UserName, credentials.Password);
        }
        #endregion
    }
}