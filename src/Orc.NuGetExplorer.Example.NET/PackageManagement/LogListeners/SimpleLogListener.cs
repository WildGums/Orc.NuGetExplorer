// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleLogListener.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example
{
    using Catel;
    using Catel.Services;
    using Models;

    public class SimpleLogListener : PackageManagerLogListenerBase
    {
        private readonly IDispatcherService _dispatcherService;

        #region Fields
        private readonly PackageManagementEcho _echo;
        #endregion

        #region Constructors
        public SimpleLogListener(INuGetLogListeningSevice nuGetLogListeningSevice,
            IEchoService echoService, IDispatcherService dispatcherService)
            : base(nuGetLogListeningSevice)
        {            
            Argument.IsNotNull(() => echoService);
            Argument.IsNotNull(() => dispatcherService);

            _dispatcherService = dispatcherService;

            _echo = echoService.GetPackageManagementEcho();
        }
        #endregion

        #region Methods
        protected override void OnInfo(object sender, NuGetLogRecordEventArgs e)
        {
            _dispatcherService.Invoke(() => _echo.Lines.Add(string.Format("Info: {0}", e.Message)), true);
        }

        protected override void OnError(object sender, NuGetLogRecordEventArgs e)
        {
            _dispatcherService.Invoke(() => _echo.Lines.Add(string.Format("Error: {0}", e.Message)), true);
        }

        protected override void OnDebug(object sender, NuGetLogRecordEventArgs e)
        {
            _dispatcherService.Invoke(() => _echo.Lines.Add(string.Format("Debug: {0}", e.Message)), true);
        }

        protected override void OnWarning(object sender, NuGetLogRecordEventArgs e)
        {
            _dispatcherService.Invoke(() => _echo.Lines.Add(string.Format("Warning: {0}", e.Message)), true);
        }
        #endregion
    }
}
