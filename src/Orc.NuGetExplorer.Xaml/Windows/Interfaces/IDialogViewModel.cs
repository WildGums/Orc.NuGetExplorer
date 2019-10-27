namespace Orc.NuGetExplorer.Windows
{
    using Catel.MVVM;

    public interface IDialogViewModel
    {
        DialogCustomization Dialog { get; }

        TaskCommand<IDialogOption> RunOption { get; }
    }
}
