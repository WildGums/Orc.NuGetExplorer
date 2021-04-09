namespace Orc.NuGetExplorer.ViewModels
{
    using Catel.MVVM;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using NuGetExplorer.Models;

    public class FeedDetailViewModel : ViewModelBase
    {
        public FeedDetailViewModel(NuGetFeed feed)
        {
            Feed = feed;
            OpenChooseLocalPathToSourceDialog = new Command(OnOpenChooseLocalPathToSourceDialogExecute, OnOpenChooseLocalPathToSourceDialogCanExecute);
        }

        [Model]
        public NuGetFeed Feed { get; set; }

        [ViewModelToModel]
        public string Name { get; set; }

        [ViewModelToModel]
        public string Source { get; set; }

        private void UpdateFeed()
        {
            if (!IsInitialized || Feed is null)
            {
                return;
            }

            // manually save model and pass forward
            Feed.ForceEndEdit();
        }

        public Command OpenChooseLocalPathToSourceDialog { get; set; }

        private void OnOpenChooseLocalPathToSourceDialogExecute()
        {
            var folderDialog = new CommonOpenFileDialog();

            folderDialog.InitialDirectory = @"C:\Users";
            folderDialog.IsFolderPicker = true;
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Source = folderDialog.FileName;
            }
        }

        private bool OnOpenChooseLocalPathToSourceDialogCanExecute()
        {
            return Feed is not null;
        }
    }
}
