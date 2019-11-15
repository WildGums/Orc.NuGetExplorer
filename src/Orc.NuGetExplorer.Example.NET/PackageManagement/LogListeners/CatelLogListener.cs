namespace Orc.NuGetExplorer.Example.PackageManagement
{
    using System;
    using Catel;
    using Catel.Logging;
    using Catel.Services;
    using Orc.NuGetExplorer.Example.Models;

    public class CatelLogListener : ILogListener
    {
        private readonly IDispatcherService _dispatcherService;

        private readonly PackageManagementEcho _echo;

        public CatelLogListener(IDispatcherService dispatcherService, IEchoService echoService)
        {
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => echoService);

            _dispatcherService = dispatcherService;

            _echo = echoService.GetPackageManagementEcho();
        }

        public bool IgnoreCatelLogging { get; set; } = true;
        public bool IsDebugEnabled { get; set; } = false;
        public bool IsInfoEnabled { get; set; } = true;
        public bool IsWarningEnabled { get; set; } = true;
        public bool IsErrorEnabled { get; set; } = true;
        public bool IsStatusEnabled { get; set; } = true;
        public TimeDisplay TimeDisplay { get; set; } = TimeDisplay.DateTime;

        public event EventHandler<LogMessageEventArgs> LogMessage;

        private void RaiseLogMessage(ILog log, string message, object extraData, LogData logData, LogEvent logEvent, DateTime time)
        {
            LogMessage?.Invoke(this, new LogMessageEventArgs(log, message, extraData, logData, logEvent, time));
        }

        public void Debug(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Echo(log, string.Format("Debug: {0}", message));
        }

        public void Error(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Echo(log, string.Format("Error: {0}", message));
        }

        public void Info(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Echo(log, string.Format("Info: {0}", message));
        }

        public void Status(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Echo(log, string.Format("Status: {0}", message));
        }

        public void Warning(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Echo(log, string.Format("Warning: {0}", message));
        }

        public void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            //
        }

        private void Echo(ILog log, string message)
        {
            if (log.IsCatelLogging)
            {
                return;
            }

            _dispatcherService.Invoke(() => _echo.Lines.Add(message), true);
        }
    }
}
