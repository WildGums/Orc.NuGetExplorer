// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoListener.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example
{
    using Catel;
    using Models;

    public class EchoListener : PackageManagementListenerBase
    {
        #region Constructors
        public EchoListener(IPackageManagementListeningService packageManagementListeningService, IEchoService echoService)
            : base(packageManagementListeningService)
        {
            Argument.IsNotNull(() => echoService);

            _echo = echoService.GetPackageManagementEcho();
        }
        #endregion

        private PackageManagementEcho _echo;
        protected override void OnPackageInstalled(object sender, NuGetPackageOperationEventArgs e)
        {
            _echo.Lines.Add(string.Format("Installed {0}", e.PackageDetails.Title));
        }

        protected override void OnPackageInstalling(object sender, NuGetPackageOperationEventArgs e)
        {
            _echo.Lines.Add(string.Format("Installing {0}", e.PackageDetails.Title));
        }

        protected override void OnPackageUninstalled(object sender, NuGetPackageOperationEventArgs e)
        {
            _echo.Lines.Add(string.Format("Uninstalled {0}", e.PackageDetails.Title));
        }

        protected override void OnPackageUninstalling(object sender, NuGetPackageOperationEventArgs e)
        {
            _echo.Lines.Add(string.Format("Uninstalling {0}", e.PackageDetails.Title));
        }
    }
}