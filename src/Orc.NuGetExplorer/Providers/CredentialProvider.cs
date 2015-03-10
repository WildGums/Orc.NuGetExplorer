// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using Catel;
    using NuGet;

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
            var credentials = _authenticationProvider.GetCredentials(uri);
            if (credentials == null)
            {
                return null;
            }

            return new NetworkCredential(credentials.UserName, credentials.Password);
        }
        #endregion
    }
}