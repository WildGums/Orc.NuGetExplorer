namespace Orc.NuGetExplorer.Windows
{
    using System.Collections.Generic;

    internal class DialogCustomization
    {
        public DialogCustomization(IEnumerable<IDialogOption> options, bool isCloseButtonAvailable)
        {
            Options = options;
            IsCloseButtonAvaialble = isCloseButtonAvailable;
        }

        public IEnumerable<IDialogOption> Options { get; }

        public bool IsCloseButtonAvaialble { get; }
    }
}
