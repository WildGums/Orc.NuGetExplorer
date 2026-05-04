namespace Orc.NuGetExplorer.Management;

using System;
using System.Xml.Linq;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using NuGet.ProjectManagement;

internal class NuGetProjectContext : INuGetProjectContext
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(NuGetProjectContext));

    public NuGetProjectContext(FileConflictAction fileConflictAction, NuGet.Common.ILogger logger)
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
                Logger.LogDebug(message, args);
                break;

            case MessageLevel.Error:
                Logger.LogError(message, args);
                break;

            case MessageLevel.Info:
                Logger.LogInformation(message, args);
                break;

            case MessageLevel.Warning:
                Logger.LogWarning(message, args);
                break;
        }
    }

    void INuGetProjectContext.Log(NuGet.Common.ILogMessage message)
    {
        switch (message.Level)
        {
            case NuGet.Common.LogLevel.Debug:
                Logger.LogDebug(FormatStringMessage(message));
                break;

            case NuGet.Common.LogLevel.Verbose:
                Logger.LogDebug(FormatStringMessage(message));
                break;

            case NuGet.Common.LogLevel.Information:
                Logger.LogInformation(FormatStringMessage(message));
                break;

            case NuGet.Common.LogLevel.Minimal:
                Logger.LogInformation(FormatStringMessage(message));
                break;

            case NuGet.Common.LogLevel.Warning:
                Logger.LogWarning(FormatStringMessage(message));
                break;

            case NuGet.Common.LogLevel.Error:
                Logger.LogError(FormatStringMessage(message));
                break;
        }
    }

    public FileConflictAction ResolveFileConflict(string message)
    {
        if (FileConflictAction == FileConflictAction.PromptUser)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Manual resolution for packages conflict is not supported in Orc.NuGetExplorer");
        }

        return FileConflictAction;
    }

    public void ReportError(string message)
    {
        Logger.LogError(message);
    }

    public void ReportError(NuGet.Common.ILogMessage message)
    {
        Logger.LogError(FormatStringMessage(message));
    }

    private static string FormatStringMessage(NuGet.Common.ILogMessage logMessage)
    {
        // For now simple write Code + Message
        return $"{logMessage.Code}: {logMessage.Message}";
    }
}
