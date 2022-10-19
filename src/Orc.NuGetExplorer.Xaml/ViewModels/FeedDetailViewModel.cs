namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using Catel.MVVM;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using NuGetExplorer.Models;

    public class FeedDetailViewModel : ViewModelBase
    {
        public FeedDetailViewModel(NuGetFeed feed)
        {
            ArgumentNullException.ThrowIfNull(feed);

            Feed = feed;
            OpenChooseLocalPathToSourceDialog = new Command(OnOpenChooseLocalPathToSourceDialogExecute, OnOpenChooseLocalPathToSourceDialogCanExecute);
        }

        [Model]
        public NuGetFeed Feed { get; set; }

        [ViewModelToModel]
        public string? Name { get; set; }

        [ViewModelToModel]
        public string? Source { get; set; }

        public Command OpenChooseLocalPathToSourceDialog { get; set; }

        private void OnOpenChooseLocalPathToSourceDialogExecute()
        {
            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.InitialDirectory = @"C:\Users";
                folderDialog.IsFolderPicker = true;

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Source = folderDialog.FileName;
                }
            }
        }

        private bool OnOpenChooseLocalPathToSourceDialogCanExecute()
        {
            return Feed is not null;
        }
    }
}
