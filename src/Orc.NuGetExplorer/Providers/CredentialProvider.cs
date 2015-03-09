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
        private readonly IAuthenticationProvider _authenticationProvider;

        public CredentialProvider(IAuthenticationProvider authenticationProvider)
        {
            Argument.IsNotNull(() => authenticationProvider);

            _authenticationProvider = authenticationProvider;
        }

        public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
        {
            switch (credentialType)
            {
                case CredentialType.ProxyCredentials:
                    return _authenticationProvider.GetProxyCredentials(uri, proxy);
                case CredentialType.RequestCredentials:
                    return _authenticationProvider.GetRequestCredentials(uri, proxy);
            }

            return null;
        }
    }
}