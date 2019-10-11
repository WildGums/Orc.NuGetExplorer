using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Models;

namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Collections;
    using Catel.MVVM;
    using NuGetExplorer.Management;
    using NuGetExplorer.Models;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProjectsViewModel : ViewModelBase
    {
        private readonly IExtensibleProjectLocator _extensiblesManager;

        public ProjectsViewModel(NuGetActionTarget projectsModel, IExtensibleProjectLocator extensiblesManager)
        {
            Argument.IsNotNull(() => extensiblesManager);
            Argument.IsNotNull(() => projectsModel);

            _extensiblesManager = extensiblesManager;
            ProjectsModel = projectsModel;
        }

        [Model]
        public NuGetActionTarget ProjectsModel { get; set; }

        public ObservableCollection<CheckableUnit<IExtensibleProject>> Projects { get; set; }

        protected override Task InitializeAsync()
        {
            if (!_extensiblesManager.IsConfigLoaded)
            {
                _extensiblesManager.RestoreStateFromConfig();
            }

            var availableProjects = _extensiblesManager.GetAllExtensibleProjects()
                .Where(x => _extensiblesManager.IsEnabled(x));

            Projects = new ObservableCollection<CheckableUnit<IExtensibleProject>>(availableProjects
                .Select(x =>
                    new CheckableUnit<IExtensibleProject>(true, x, NotifyOnProjectSelectionChanged)));

            Projects.ForEach(x => ProjectsModel.Add(x.Value));

            return base.InitializeAsync();
        }

        private void NotifyOnProjectSelectionChanged(bool isSelected, IExtensibleProject project)
        {
            if (isSelected)
            {
                ProjectsModel.Add(project);
            }
            else
            {
                ProjectsModel.Remove(project);
            }
        }
    }
}
