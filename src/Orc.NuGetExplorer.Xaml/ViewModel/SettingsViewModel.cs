namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;
    using NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;

    internal class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(ExplorerSettingsContainer settings)
        {
            Argument.IsNotNull(() => settings);
            Settings = settings;
        }

        public SettingsViewModel(IModelProvider<ExplorerSettingsContainer> settingsProvider)
        {
            Argument.IsNotNull(() => settingsProvider);
            Settings = settingsProvider.Model;
        }

        [Model(SupportIEditableObject = false)]
        [Expose("NuGetFeeds")]
        public ExplorerSettingsContainer Settings { get; set; }

    }
}
