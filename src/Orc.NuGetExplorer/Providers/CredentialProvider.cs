// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
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
            var credentials = _authenticationProvider.GetCredentials(uri);
            if (credentials == null)
            {
                return null;
            }
            
            return new NetworkCredential(credentials.UserName, credentials.Password);
        }
    }
}