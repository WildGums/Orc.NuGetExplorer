// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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