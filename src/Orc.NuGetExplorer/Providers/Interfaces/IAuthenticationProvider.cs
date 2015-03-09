// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;

    public interface IAuthenticationProvider
    {
        #region Methods
        ICredentials GetProxyCredentials(Uri uri, IWebProxy proxy);
        ICredentials GetRequestCredentials(Uri uri, IWebProxy proxy);
        #endregion
    }
}