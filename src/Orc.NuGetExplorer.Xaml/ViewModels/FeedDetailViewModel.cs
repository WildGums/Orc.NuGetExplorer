namespace Orc.NuGetExplorer.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Catel;
    using Catel.MVVM;
    using Catel.Services;
    using NuGetExplorer.Models;

    public class FeedDetailViewModel : ViewModelBase
    {
        private readonly ISelectDirectoryService _selectDirectoryService;

        public FeedDetailViewModel(NuGetFeed feed, ISelectDirectoryService selectDirectoryService)
        {
            Argument.IsNotNull(() => feed);
            Argument.IsNotNull(() => selectDirectoryService);

            Feed = feed;
            _selectDirectoryService = selectDirectoryService;
            OpenChooseLocalPathToSourceDialog = new TaskCommand(OnOpenChooseLocalPathToSourceDialogExecuteAsync, OnOpenChooseLocalPathToSourceDialogCanExecute);
        }

        [Model]
        public NuGetFeed Feed { get; set; }

        [ViewModelToModel]
        public string Name { get; set; }

        [ViewModelToModel]
        public string Source { get; set; }

        public ICommand OpenChooseLocalPathToSourceDialog { get; set; }

        private async Task OnOpenChooseLocalPathToSourceDialogExecuteAsync()
        {
            var directorySelectionContext = new DetermineDirectoryContext
            {
                ShowNewFolderButton = false,
            };
            var result = await _selectDirectoryService.DetermineDirectoryAsync(directorySelectionContext);
            if (result.Result)
            {
                Source = result.DirectoryName;
            }
        }

        private bool OnOpenChooseLocalPathToSourceDialogCanExecute()
        {
            return Feed is not null;
        }
    }
}
