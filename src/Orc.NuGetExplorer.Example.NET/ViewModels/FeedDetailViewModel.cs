using Orc.NuGetExplorer.Models;
using Orc.NuGetExplorer.Providers;

namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.MVVM;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using NuGetExplorer.Models;
    using NuGetExplorer.Providers;
    using System.Threading.Tasks;

    public class FeedDetailViewModel : ViewModelBase
    {
        private readonly IModelProvider<NuGetFeed> _modelProvider;

        public FeedDetailViewModel(NuGetFeed feed, IModelProvider<NuGetFeed> modelProvider)
        {
            Argument.IsNotNull(() => feed);
            Argument.IsNotNull(() => modelProvider);

            _modelProvider = modelProvider;

            //work with model clone

            Feed = feed.Clone();

            UpdateFeed = new Command(OnUpdateFeedExecute, OnUpdateFeedCanExecute);
            OpenChooseLocalPathToSourceDialog = new Command(OnOpenChooseLocalPathToSourceDialogExecute, OnOpenChooseLocalPathToSourceDialogCanExecute);
        }

        [Model]
        public NuGetFeed Feed { get; set; }

        [ViewModelToModel]
        public string Name { get; set; }

        [ViewModelToModel]
        public string Source { get; set; }

        public Command UpdateFeed { get; set; }

        private void OnUpdateFeedExecute()
        {
            //manually save model and pass forward
            Feed.ForceEndEdit();
            _modelProvider.Model = Feed;
        }

        private bool OnUpdateFeedCanExecute()
        {
            return Feed != null;
        }

        public Command OpenChooseLocalPathToSourceDialog { get; set; }

        private void OnOpenChooseLocalPathToSourceDialogExecute()
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();

            folderDialog.InitialDirectory = @"C:\Users";
            folderDialog.IsFolderPicker = true;
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Source = folderDialog.FileName;
            }
        }

        private bool OnOpenChooseLocalPathToSourceDialogCanExecute()
        {
            return Feed != null;
        }

        protected override Task<bool> SaveAsync()
        {
            return Task.FromResult(true);
        }
    }
}
