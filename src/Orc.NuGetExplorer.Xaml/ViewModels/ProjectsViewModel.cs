namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Collections;
    using Catel.MVVM;
    using NuGetExplorer.Management;
    using Orc.NuGetExplorer;

    internal class ProjectsViewModel : ViewModelBase
    {
        private readonly IExtensibleProjectLocator _extensiblesManager;

        public ProjectsViewModel(NuGetActionTarget projectsModel, IExtensibleProjectLocator extensiblesManager)
        {
            ArgumentNullException.ThrowIfNull(projectsModel);
            ArgumentNullException.ThrowIfNull(extensiblesManager);

            _extensiblesManager = extensiblesManager;
            ProjectsModel = projectsModel;
        }

        [Model(SupportIEditableObject = false)]
        public NuGetActionTarget ProjectsModel { get; set; }

        public ObservableCollection<CheckableUnit<IExtensibleProject>> Projects { get; set; } = new();

        protected override Task InitializeAsync()
        {
            if (!_extensiblesManager.IsConfigLoaded)
            {
                _extensiblesManager.RestoreStateFromConfig();
            }

            var availableProjects = _extensiblesManager.GetAllExtensibleProjects();

            Projects = new ObservableCollection<CheckableUnit<IExtensibleProject>>(availableProjects
                .Select(x =>
                    new CheckableUnit<IExtensibleProject>(true, x, NotifyOnProjectSelectionChanged)));

            Projects.ForEach(x => ProjectsModel.Add(x.Value));

            return base.InitializeAsync();
        }

        private void NotifyOnProjectSelectionChanged(IExtensibleProject project, bool isSelected)
        {
            ArgumentNullException.ThrowIfNull(project);

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
