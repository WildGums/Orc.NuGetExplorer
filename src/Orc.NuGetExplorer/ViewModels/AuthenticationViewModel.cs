// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using Catel.Fody;
    using Catel.MVVM;

    public class AuthenticationViewModel : ViewModelBase
    {
        #region Constructors
        public AuthenticationViewModel(AuthenticationCredentials authenticationCredentials)
        {
            AuthenticationCredentials = authenticationCredentials;
        }
        #endregion

        #region Properties
        [Model]
        [Expose("Password")]
        [Expose("RememberMe")]
        [Expose("UserName")]
        public AuthenticationCredentials AuthenticationCredentials { get; private set; }

        private void OnPasswordChanged()
        {
        }
        #endregion
    }
}