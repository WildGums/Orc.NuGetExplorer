namespace Orc.NuGetExplorer.Management
{
    using System;
    using System.Xml.Linq;
    using Catel;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Packaging;
    using NuGet.ProjectManagement;
    using NuGetExplorer.Windows.Dialogs;
    using Orc.NuGetExplorer.Windows;

    internal class NuGetProjectContext : INuGetProjectContext
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ILogger _nugetLogger;

        private readonly IMessageDialogService _messageDialogService;

        public NuGetProjectContext(FileConflictAction fileConflictAction, IMessageDialogService messageDialogService, ILogger logger)
        {
            Argument.IsNotNull(() => messageDialogService);
            Argument.IsNotNull(() => logger);

            FileConflictAction = fileConflictAction;

            _messageDialogService = messageDialogService;
            _nugetLogger = logger;
        }

        public PackageExtractionContext PackageExtractionContext { get; set; }

        public ISourceControlManagerProvider SourceControlManagerProvider { get; }

        public ExecutionContext ExecutionContext { get; }

        public FileConflictAction FileConflictAction { get; private set; }

        public XDocument OriginalPackagesConfig { get; set; }

        public NuGetActionType ActionType { get; set; }

        public Guid OperationId { get; set; }

        void INuGetProjectContext.Log(MessageLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case MessageLevel.Debug:
                    Log.Debug(message);
                    break;

                case MessageLevel.Error:
                    Log.Error(message);
                    break;

                case MessageLevel.Info:
                    Log.Info(message);
                    break;

                case MessageLevel.Warning:
                    Log.Warning(message);
                    break;
            }
        }

        public void ReportError(string message)
        {
            Log.Error(message);
        }

        public FileConflictAction ResolveFileConflict(string message)
        {
            if (FileConflictAction == FileConflictAction.PromptUser)
            {
                var resolution = ShowConflictPrompt(message);

                FileConflictAction = resolution;
            }

            return FileConflictAction;
        }

        private FileConflictAction ShowConflictPrompt(string message)
        {

            var result = _messageDialogService.ShowDialog<FileConflictAction>(NuGetExplorer.Constants.PackageInstallationConflictMessage,
                 message,
                 false,
                 FileConflictDialogOption.OverWrite,
                 FileConflictDialogOption.OverWriteAll,
                 FileConflictDialogOption.Ignore,
                 FileConflictDialogOption.IgnoreAll
             );

            return result;
        }
    }
}
