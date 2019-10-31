namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using NuGetExplorer.Management;

    internal class ExtensiblesViewModel : ViewModelBase
    {
        private readonly IExtensibleProjectLocator _extensiblesManager;

        public ExtensiblesViewModel(IExtensibleProjectLocator extensiblesManager)
        {
            Argument.IsNotNull(() => extensiblesManager);

            _extensiblesManager = extensiblesManager;

            Title = "Project extensions";
        }

        protected override Task InitializeAsync()
        {
            var registeredProjects = _extensiblesManager.GetAllExtensibleProjects();

            if (!_extensiblesManager.IsConfigLoaded)
            {
                _extensiblesManager.RestoreStateFromConfig();
            }

            ExtensiblesCollection = new ObservableCollection<CheckableUnit<IExtensibleProject>>(
                registeredProjects
                .Select(
                    x => new CheckableUnit<IExtensibleProject>(_extensiblesManager.IsEnabled(x), x, ExtensibleProjectStatusChange))
                );

            return base.InitializeAsync();
        }

        public void ExtensibleProjectStatusChange(bool isShouldBeEnabled, IExtensibleProject project)
        {
            Argument.IsNotNull(() => project);

            if (isShouldBeEnabled)
            {
                _extensiblesManager.Enable(project);
            }
            else
            {
                _extensiblesManager.Disable(project);
            }
        }

        public ObservableCollection<CheckableUnit<IExtensibleProject>> ExtensiblesCollection { get; set; }

        protected override Task OnClosingAsync()
        {
            _extensiblesManager.PersistChanges();
            return base.OnClosingAsync();
        }
    }
}
