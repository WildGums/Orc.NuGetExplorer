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
        private readonly IAutentificationProvider _autentificationProvider;

        public CredentialProvider(IAutentificationProvider autentificationProvider)
        {
            Argument.IsNotNull(() => autentificationProvider);

            _autentificationProvider = autentificationProvider;
        }

        public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
        {
            switch (credentialType)
            {
                case CredentialType.ProxyCredentials:
                    return _autentificationProvider.GetProxyCredentials(uri, proxy);
                case CredentialType.RequestCredentials:
                    return _autentificationProvider.GetRequestCredentials(uri, proxy);
            }

            return null;
        }
    }
}