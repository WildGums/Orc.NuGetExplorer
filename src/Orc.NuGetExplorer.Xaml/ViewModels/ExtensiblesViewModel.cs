namespace Orc.NuGetExplorer.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Catel.MVVM;
using Catel.Services;
using NuGetExplorer.Management;

internal class ExtensiblesViewModel : ViewModelBase
{
    private readonly IExtensibleProjectLocator _extensiblesManager;

    public ExtensiblesViewModel(IExtensibleProjectLocator extensiblesManager,
        ILanguageService languageService,
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _extensiblesManager = extensiblesManager;

        ExtensiblesCollection = new();

        Title = languageService.GetRequiredString("NuGetExplorer_ExtensiblesViewModel_Title");
    }

    protected override Task InitializeAsync()
    {
        var registeredProjects = _extensiblesManager.GetAllExtensibleProjects(onlyEnabled: false);

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

    public void ExtensibleProjectStatusChange(IExtensibleProject project, bool isShouldBeEnabled)
    {
        ArgumentNullException.ThrowIfNull(project);

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
