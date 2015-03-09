// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;

    internal class AuthenticationProvider : IAuthenticationProvider
    {
        #region Methods
        public ICredentials GetProxyCredentials(Uri uri, IWebProxy proxy)
        {
            return new NetworkCredential();
        }

        public ICredentials GetRequestCredentials(Uri uri, IWebProxy proxy)
        {
            return new NetworkCredential();
        }

        #endregion
    }
}