// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using ViewModels;

    internal class AuthenticationProvider : IAuthenticationProvider
    {
        #region Fields
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IDispatcherService _dispatcherService;
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public AuthenticationProvider(IUIVisualizerService uiVisualizerService, IDispatcherService dispatcherService,
            IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => dispatcherService);

            _uiVisualizerService = uiVisualizerService;
            _dispatcherService = dispatcherService;
            _pleaseWaitService = pleaseWaitService;
        }
        #endregion

        #region Methods
        public AuthenticationCredentials GetCredentials(Uri uri)
        {
            bool? result = null;

            try
            {
                _pleaseWaitService.Hide();

                var credentials = new AuthenticationCredentials(uri);

                _dispatcherService.Invoke(() => result = _uiVisualizerService.ShowDialog<AuthenticationViewModel>(credentials));

                _pleaseWaitService.Show();

                if (result != null && result.Value)
                {
                    return credentials;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        #endregion
    }
}