// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Catel;
    using NuGet;

    internal class CredentialProvider : ICredentialProvider
    {
        #region Fields
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly IList<Uri> _cancelledUris;
        #endregion

        #region Constructors
        public CredentialProvider(IAuthenticationProvider authenticationProvider)
        {
            Argument.IsNotNull(() => authenticationProvider);

            _authenticationProvider = authenticationProvider;

            _cancelledUris = new List<Uri>();
        }
        #endregion

        #region Methods
        public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
        {
            if (_cancelledUris.Contains(uri))
            {
                return null;
            }

            var credentials = _authenticationProvider.GetCredentials(uri, retrying);
            if (credentials == null)
            {
                _cancelledUris.Add(uri);
                return null;
            }

            return new NetworkCredential(credentials.UserName, credentials.Password);
        }
        #endregion
    }
}