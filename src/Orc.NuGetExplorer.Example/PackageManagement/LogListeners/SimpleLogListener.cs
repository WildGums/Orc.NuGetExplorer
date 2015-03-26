// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleLogListener.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example
{
    using Catel;
    using Models;

    public class SimpleLogListener : PackageManagerLogListenerBase
    {
        #region Fields
        private readonly PackageManagementEcho _echo;
        #endregion

        #region Constructors
        public SimpleLogListener(INuGetLogListeningSevice nuGetLogListeningSevice,
            IEchoService echoService)
            : base( nuGetLogListeningSevice)
        {
            Argument.IsNotNull(() => echoService);

            _echo = echoService.GetPackageManagementEcho();
        }
        #endregion

        #region Methods
        protected override void OnInfo(object sender, NuGetLogRecordEventArgs e)
        {
            Argument.IsNotNull(() => e);

            _echo.Lines.Add(string.Format("Info: {0}", e.Message));
        }

        protected override void OnError(object sender, NuGetLogRecordEventArgs e)
        {
            Argument.IsNotNull(() => e);

            _echo.Lines.Add(string.Format("Error: {0}", e.Message));
        }

        protected override void OnDebug(object sender, NuGetLogRecordEventArgs e)
        {
            Argument.IsNotNull(() => e);

            _echo.Lines.Add(string.Format("Debug: {0}", e.Message));
        }

        protected override void OnWarning(object sender, NuGetLogRecordEventArgs e)
        {
            Argument.IsNotNull(() => e);

            _echo.Lines.Add(string.Format("Warning: {0}", e.Message));
        }
        #endregion
    }
}