namespace Orc.NuGetExplorer.Logging
{
    using System;
    using System.Diagnostics;
    using Catel.Logging;
    using Orc.Controls;
    using Orc.NuGetExplorer;

    public class NuGetLogListener : PackageManagerLogListenerBase, ILogListener
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly LogData EmptyAdditionalData = new LogData();

        public NuGetLogListener(INuGetLogListeningSevice nuGetLogListeningSevice) : base(nuGetLogListeningSevice)
        {
        }

        public bool IgnoreCatelLogging { get; set; }
        public bool IsDebugEnabled { get; set; }
        public bool IsInfoEnabled { get; set; }
        public bool IsWarningEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public bool IsStatusEnabled { get; set; }
        public TimeDisplay TimeDisplay { get; set; }

        public event EventHandler<LogMessageEventArgs> LogMessage;

        private void RaiseLogMessage(LogMessageEventArgs args)
        {
            LogMessage?.Invoke(this, args);
        }

        public void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            RaiseLogMessage(CreateMessageEventArgs(log, message, extraData, logData, logEvent, time));
        }

        public void Debug(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Write(log, message, LogEvent.Debug, extraData, logData, time);
        }

        public void Info(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Write(log, message, LogEvent.Info, extraData, logData, time);
        }

        public void Warning(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Write(log, message, LogEvent.Warning, extraData, logData, time);
        }

        public void Error(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Write(log, message, LogEvent.Error, extraData, logData, time);
        }

        public void Status(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            Write(log, message, LogEvent.Status, extraData, logData, time);
        }

        //on messages from NuGet logger
        protected override void OnDebug(object sender, NuGetLogRecordEventArgs e)
        {
            Debug(Log, e.Message, null, EmptyAdditionalData, DateTime.Now);
        }

        protected override void OnError(object sender, NuGetLogRecordEventArgs e)
        {
            Error(Log, e.Message, null, EmptyAdditionalData, DateTime.Now);
        }

        protected override void OnInfo(object sender, NuGetLogRecordEventArgs e)
        {
            Info(Log, e.Message, null, EmptyAdditionalData, DateTime.Now);
        }

        protected override void OnWarning(object sender, NuGetLogRecordEventArgs e)
        {
            Warning(Log, e.Message, null, EmptyAdditionalData, DateTime.Now);
        }

        private LogMessageEventArgs CreateMessageEventArgs(ILog log, string message, object extraData, LogData logData, LogEvent logEvent, DateTime time)
        {
            return new LogMessageEventArgs(log, message, extraData, logData, logEvent, time);
        }

    }
}
