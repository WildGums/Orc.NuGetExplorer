// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationScope.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2016 Wild Gums. All rights reserved.
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