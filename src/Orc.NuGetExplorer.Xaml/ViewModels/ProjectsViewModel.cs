namespace Orc.NuGetExplorer.ViewModels;

using System;
using System.Linq;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.MVVM;
using NuGetExplorer.Management;
using Orc.NuGetExplorer;

internal class ProjectsViewModel : FeaturedViewModelBase
{
    private readonly IExtensibleProjectLocator _extensiblesManager;
    private readonly IServiceProvider _serviceProvider;

    public ProjectsViewModel(NuGetActionTarget projectsModel, 
        IExtensibleProjectLocator extensiblesManager,
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _extensiblesManager = extensiblesManager;
        _serviceProvider = serviceProvider;

        ProjectsModel = projectsModel;
    }

    [Model(SupportIEditableObject = false)]
    public NuGetActionTarget ProjectsModel { get; set; }

    public System.Collections.ObjectModel.ObservableCollection<CheckableUnit<IExtensibleProject>> Projects { get; set; } = new();

    protected override Task InitializeAsync()
    {
        if (!_extensiblesManager.IsConfigLoaded)
        {
            _extensiblesManager.RestoreStateFromConfig();
        }

        var availableProjects = _extensiblesManager.GetAllExtensibleProjects();

        Projects = new System.Collections.ObjectModel.ObservableCollection<CheckableUnit<IExtensibleProject>>(availableProjects
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
