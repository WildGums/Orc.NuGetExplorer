// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationScope.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Scopes
{
    using Catel;

    public class AuthenticationScope : Disposable
    {
        private readonly bool _canPromptForAuthentication;

        public AuthenticationScope(bool? canPromptForAuthentication = null)
        {
            _canPromptForAuthentication = canPromptForAuthentication ?? true;
        }

        public bool CanPromptForAuthentication
        {
            get { return !HasPromptedForAuthentication && _canPromptForAuthentication; }
        }

        public bool HasPromptedForAuthentication { get; set; }
    }
}