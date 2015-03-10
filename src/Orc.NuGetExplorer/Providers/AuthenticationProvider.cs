// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.Services;
    using Native;
    using ViewModels;

    internal class AuthenticationProvider : IAuthenticationProvider
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
            Log.Debug("Requesting credentials for '{0}'", uri);

            bool? result = null;

            _pleaseWaitService.Hide();

            var credentials = new AuthenticationCredentials(uri);

            /*try
            {
                _dispatcherService.Invoke(() =>
                {
                    try
                    {
                        var credentialsPrompter = new CredentialsPrompter();
                        credentialsPrompter.UserName = string.Empty;
                        credentialsPrompter.Password = string.Empty;
                        credentialsPrompter.WindowTitle = "prompt credentials";
                        credentialsPrompter.ShowDialog();
                    }
                    catch (Exception)
                    {
                        
                    }
                });

            }
            catch (Exception)
            {

                throw;
            }*/


            _dispatcherService.Invoke(() => result = _uiVisualizerService.ShowDialog<AuthenticationViewModel>(credentials));

            _pleaseWaitService.Show();

            if (result != null && result.Value)
            {
                Log.Debug("Successfully requested credentials for '{0}'", uri);

                return credentials;
            }

            Log.Debug("Failed to request credentials for '{0}'", uri);

            return null;
        }
        #endregion
    }
}