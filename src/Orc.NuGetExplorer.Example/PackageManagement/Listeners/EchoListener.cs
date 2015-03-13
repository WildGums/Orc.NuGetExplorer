// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoListener.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example
{
    using Catel;
    using Models;

    public class EchoListener : PackageManagerListenerBase
    {
        #region Fields
        private readonly PackageManagementEcho _echo;
        #endregion

        #region Constructors
        public EchoListener(IPackageManagerListeningService packageManagerListeningService, INuGetLogListeningSevice nuGetLogListeningSevice,
            IEchoService echoService)
            : base(packageManagerListeningService, nuGetLogListeningSevice)
        {
            Argument.IsNotNull(() => echoService);

            _echo = echoService.GetPackageManagementEcho();
        }
        #endregion

        #region Methods
        protected override void OnPackageInstalled(object sender, NuGetPackageOperationEventArgs e)
        {
            _echo.Lines.Add(string.Format("Installed {0}", e.PackageDetails.FullName));
        }

        protected override void OnPackageInstalling(object sender, NuGetPackageOperationEventArgs e)
        {
            _echo.Lines.Add(string.Format("Installing {0}", e.PackageDetails.FullName));
        }

        protected override void OnPackageUninstalled(object sender, NuGetPackageOperationEventArgs e)
        {
            _echo.Lines.Add(string.Format("Uninstalled {0}", e.PackageDetails.FullName));
        }

        protected override void OnPackageUninstalling(object sender, NuGetPackageOperationEventArgs e)
        {
            _echo.Lines.Add(string.Format("Uninstalling {0}", e.PackageDetails.FullName));
        }

        protected override void OnInfo(object sender, NuGetLogRecordEventArgs e)
        {
            _echo.Lines.Add(string.Format("Info: {0}", e.Message));
        }

        protected override void OnError(object sender, NuGetLogRecordEventArgs e)
        {
            _echo.Lines.Add(string.Format("Error: {0}", e.Message));
        }

        protected override void OnDebug(object sender, NuGetLogRecordEventArgs e)
        {
            _echo.Lines.Add(string.Format("Debug: {0}", e.Message));
        }

        protected override void OnWarning(object sender, NuGetLogRecordEventArgs e)
        {
            _echo.Lines.Add(string.Format("Warning: {0}", e.Message));
        }
        #endregion
    }
}