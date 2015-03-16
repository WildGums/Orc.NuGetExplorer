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
            Password = string.Empty;
        }
        #endregion

        #region Properties
        public string Host { get; private set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool StoreCredentials { get; set; }
        #endregion
    }
}