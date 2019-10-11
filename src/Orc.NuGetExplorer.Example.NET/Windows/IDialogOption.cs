namespace Orc.NuGetExplorer.Windows
{
    using System;

    public interface IDialogOption
    {
        /// <summary>
        ///  Is this dialog option replaces and act as standart window 'ok' button,
        /// </summary>
        bool IsOkBehavior { get; }

        /// <summary>
        /// Is this dialog option replaces and act as standart window 'apply' button
        /// </summary>
        bool IsApplyBehavior { get; }

        /// <summary>
        /// Is this dialog option replaces and act as standart window ''
        /// </summary>
        bool IsCancelBehavior { get; }

        bool IsDefault { get; }

        string Caption { get; }
    }

    public interface IDialogOption<T> : IDialogOption
    {
        /// <summary>
        /// Additional action performed when dialog option was selected
        /// </summary>
        Func<T> DialogCallback { get; }
    }
}
