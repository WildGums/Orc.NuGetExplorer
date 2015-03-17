// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationSilencerService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public interface IAuthenticationSilencerService
    {
        #region Properties
        bool IsAuthenticationRequired { get; set; }
        #endregion

        #region Methods
        IDisposable AuthenticationRequiredScope(bool authenticateIfRequired = true);
        #endregion
    }
}