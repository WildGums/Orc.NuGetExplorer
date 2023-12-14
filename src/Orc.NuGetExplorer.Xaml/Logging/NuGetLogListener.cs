namespace Orc.NuGetExplorer.Logging;

using System;
using Catel.Logging;
using Orc.NuGetExplorer;

public class NuGetLogListener : PackageManagerLogListenerBase, ILogListener
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();
    private static readonly LogData EmptyAdditionalData = new();

    public NuGetLogListener(INuGetLogListeningSevice nuGetLogListeningSevice)
        : base(nuGetLogListeningSevice)
    {
    }

    public bool IgnoreCatelLogging { get; set; }
    public bool IsDebugEnabled { get; set; }
    public bool IsInfoEnabled { get; set; }
    public bool IsWarningEnabled { get; set; }
    public bool IsErrorEnabled { get; set; }
    public bool IsStatusEnabled { get; set; }
    public TimeDisplay TimeDisplay { get; set; }

    public event EventHandler<LogMessageEventArgs>? LogMessage;

    private void RaiseLogMessage(LogMessageEventArgs args)
    {
        LogMessage?.Invoke(this, args);
    }

    public void Write(ILog log, string message, LogEvent logEvent, object? extraData, LogData? logData, DateTime time)
    {
        WriteInternal(log, message, logEvent, logData ?? new(), time);
    }

    private void WriteInternal(ILog log, string message, LogEvent logEvent, LogData logData, DateTime time)
    {
        ArgumentNullException.ThrowIfNull(log);

        var eventArgs = new LogMessageEventArgs(log, message, null, logData, logEvent, time);
        RaiseLogMessage(eventArgs);
    }

    public void Debug(ILog log, string message, LogData logData, DateTime time)
    {
        WriteInternal(log, message, LogEvent.Debug, logData, time);
    }

    public void Info(ILog log, string message, LogData logData, DateTime time)
    {
        WriteInternal(log, message, LogEvent.Info, logData, time);
    }

    public void Warning(ILog log, string message, LogData logData, DateTime time)
    {
        WriteInternal(log, message, LogEvent.Warning, logData, time);
    }

    public void Error(ILog log, string message, LogData logData, DateTime time)
    {
        WriteInternal(log, message, LogEvent.Error, logData, time);
    }

    public void Status(ILog log, string message, LogData logData, DateTime time)
    {
        WriteInternal(log, message, LogEvent.Status, logData, time);
    }

    protected override void OnDebug(object? sender, NuGetLogRecordEventArgs e)
    {
        Debug(Log, e.Message, EmptyAdditionalData, DateTime.Now);
    }

    protected override void OnError(object? sender, NuGetLogRecordEventArgs e)
    {
        Error(Log, e.Message, EmptyAdditionalData, DateTime.Now);
    }

    protected override void OnInfo(object? sender, NuGetLogRecordEventArgs e)
    {
        Info(Log, e.Message, EmptyAdditionalData, DateTime.Now);
    }

    protected override void OnWarning(object? sender, NuGetLogRecordEventArgs e)
    {
        Warning(Log, e.Message, EmptyAdditionalData, DateTime.Now);
    }
}