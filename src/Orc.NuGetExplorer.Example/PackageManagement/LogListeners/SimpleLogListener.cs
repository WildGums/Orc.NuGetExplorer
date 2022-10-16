namespace Orc.NuGetExplorer.Example
{
    using Catel.Services;
    using Models;

    public class SimpleLogListener : PackageManagerLogListenerBase
    {
        private readonly IDispatcherService _dispatcherService;
        private readonly PackageManagementEcho _echo;

        public SimpleLogListener(INuGetLogListeningSevice nuGetLogListeningSevice,
            IEchoService echoService, IDispatcherService dispatcherService)
            : base(nuGetLogListeningSevice)
        {
            ArgumentNullException.ThrowIfNull(echoService);
            ArgumentNullException.ThrowIfNull(dispatcherService);

            _dispatcherService = dispatcherService;

            _echo = echoService.GetPackageManagementEcho();
        }

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
    }
}
