namespace Orc.NuGetExplorer.Windows
{
    using Catel.MVVM;

    internal interface IDialogViewModel
    {
        DialogCustomization Dialog { get; }

        TaskCommand<IDialogOption> RunOption { get; }
    }
}
