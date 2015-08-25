// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;

    internal interface IAuthenticationProvider
    {
        #region Methods
        Task<AuthenticationCredentials> GetCredentialsAsync(Uri uri, bool previousCredentialsFailed);
        #endregion
    }
}