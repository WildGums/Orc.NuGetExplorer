// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationCredentials.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Data;

    public class AuthenticationCredentials : ModelBase
    {
        #region Constructors
        public AuthenticationCredentials(Uri uri)
        {
            Argument.IsNotNull(() => uri);

            Host = uri.Host;
        }
        #endregion

        #region Properties
        public string Host { get; private set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string UserName { get; set; }
        #endregion
    }
}