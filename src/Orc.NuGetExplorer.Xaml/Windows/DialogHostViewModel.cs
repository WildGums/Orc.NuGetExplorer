namespace Orc.NuGetExplorer.Windows
{
    using System.Threading.Tasks;
    using Catel.MVVM;

    public class DialogHostViewModel : ViewModelBase, IDialogViewModel
    {
        public DialogHostViewModel(DialogCustomization options, DialogResult result, string title, string message)
        {
            Dialog = options;
            Title = title;

            //TODO
            //provide a way to set message as hostable content
            RunOption = new TaskCommand<IDialogOption>(RunOptionExecute);

            Result = result;
        }

        public string Message { get; set; }

        public DialogCustomization Dialog { get; }

        public TaskCommand<IDialogOption> RunOption { get; set; }

        private async Task RunOptionExecute(IDialogOption option)
        {
            Result.SetResult(option);
        }

        public DialogResult Result { get; }
    }
}
