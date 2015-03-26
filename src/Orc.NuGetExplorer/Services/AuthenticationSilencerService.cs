// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationSilencerService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;

    internal class AuthenticationSilencerService : IAuthenticationSilencerService
    {
        #region Properties
        public bool IsAuthenticationRequired { get; set; }
        #endregion

        #region Methods
        public IDisposable AuthenticationRequiredScope(bool authenticateIfRequired = true)
        {
            return new DisposableToken<bool>(IsAuthenticationRequired, token => SetIsAuthenticationRequired(authenticateIfRequired),
                token => SetIsAuthenticationRequired(token.Instance));
        }

        private void SetIsAuthenticationRequired(bool authenticateIfRequired)
        {
            IsAuthenticationRequired = authenticateIfRequired;
        }
        #endregion
    }
}