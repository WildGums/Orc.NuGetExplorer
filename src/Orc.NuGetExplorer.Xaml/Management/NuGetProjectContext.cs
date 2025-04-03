namespace Orc.NuGetExplorer.Management;

using System;
using System.Xml.Linq;
using Catel.Logging;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.ProjectManagement;

internal class NuGetProjectContext : INuGetProjectContext
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    public NuGetProjectContext(FileConflictAction fileConflictAction, ILogger logger)
    {
        FileConflictAction = fileConflictAction;
    }

    public PackageExtractionContext? PackageExtractionContext { get; set; }

    public ISourceControlManagerProvider SourceControlManagerProvider => throw new NotSupportedException();

    public ExecutionContext ExecutionContext => throw new NotSupportedException();

    public FileConflictAction FileConflictAction { get; private set; }

    public XDocument? OriginalPackagesConfig { get; set; }

    public NuGetActionType ActionType { get; set; }

    public Guid OperationId { get; set; }

    void INuGetProjectContext.Log(MessageLevel level, string message, params object[] args)
    {
        switch (level)
        {
            case MessageLevel.Debug:
                Log.Debug(string.Format(message, args));
                break;

            case MessageLevel.Error:
                Log.Error(string.Format(message, args));
                break;

            case MessageLevel.Info:
                Log.Info(string.Format(message, args));
                break;

            case MessageLevel.Warning:
                Log.Warning(string.Format(message, args));
                break;
        }
    }

    void INuGetProjectContext.Log(ILogMessage message)
    {
        switch (message.Level)
        {
            case LogLevel.Debug:
                Log.Debug(FormatStringMessage(message));
                break;

            case LogLevel.Verbose:
                Log.Debug(FormatStringMessage(message));
                break;

            case LogLevel.Information:
                Log.Info(FormatStringMessage(message));
                break;

            case LogLevel.Minimal:
                Log.Info(FormatStringMessage(message));
                break;

            case LogLevel.Warning:
                Log.Warning(FormatStringMessage(message));
                break;

            case LogLevel.Error:
                Log.Error(FormatStringMessage(message));
                break;
        }
    }

    public FileConflictAction ResolveFileConflict(string message)
    {
        if (FileConflictAction == FileConflictAction.PromptUser)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>("Manual resolution for packages conflict is not supported in Orc.NuGetExplorer");
        }

        return FileConflictAction;
    }

    public void ReportError(string message)
    {
        Log.Error(message);
    }

    public void ReportError(ILogMessage message)
    {
        Log.Error(FormatStringMessage(message));
    }

    private static string FormatStringMessage(ILogMessage logMessage)
    {
        // For now simple write Code + Message
        return $"{logMessage.Code}: {logMessage.Message}";
    }
}
