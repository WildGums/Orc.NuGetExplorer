// --------------------------------------------------------------------------------------------------------------------
// <copyright file="autentificationProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;

    internal class NullAutentificationProvider : IAutentificationProvider
    {
        #region Methods
        public ICredentials GetProxyCredentials(Uri uri, IWebProxy proxy)
        {
            return null;
        }

        public ICredentials GetRequestCredentials(Uri uri, IWebProxy proxy)
        {
            return null;
        }

        #endregion
    }
}