using Orc.NuGetExplorer.Models;

namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;
    using NuGetExplorer.Models;

    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(ExplorerSettingsContainer settings)
        {
            Argument.IsNotNull(() => settings);
            Settings = settings;
        }

        [Model(SupportIEditableObject = false)]
        [Expose("NuGetFeeds")]
        public ExplorerSettingsContainer Settings { get; set; }

    }
}
