namespace Orc.NuGetExplorer.Windows.Dialogs
{
    using System;
    using NuGet.ProjectManagement;

    internal class FileConflictDialogOption : IDialogOption<FileConflictAction>
    {
        public FileConflictDialogOption(Func<FileConflictAction> optionCallback)
        {
            DialogCallback = optionCallback;
        }

        public bool IsOkBehavior { get; set; }

        public bool IsApplyBehavior { get; set; }

        public bool IsCancelBehavior { get; set; }

        public bool IsDefault { get; set; }

        public string Caption { get; set; }

        public Func<FileConflictAction> DialogCallback { get; private set; }

        public readonly static IDialogOption Ignore = new FileConflictDialogOption(() => FileConflictAction.Ignore)
        {
            Caption = "Ignore",
            IsCancelBehavior = true
        };

        public readonly static IDialogOption IgnoreAll = new FileConflictDialogOption(() => FileConflictAction.IgnoreAll)
        {
            Caption = "Ignore All",
            IsCancelBehavior = true
        };

        public readonly static IDialogOption OverWrite = new FileConflictDialogOption(() => FileConflictAction.Overwrite)
        {
            Caption = "Overwrite",
            IsOkBehavior = true
        };

        public readonly static IDialogOption OverWriteAll = new FileConflictDialogOption(() => FileConflictAction.OverwriteAll)
        {
            Caption = "Overwrite All",
            IsOkBehavior = true
        };
    }
}
