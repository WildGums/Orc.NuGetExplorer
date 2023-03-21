namespace Orc.NuGetExplorer.Management;

using System.Collections.Generic;

public interface IExtensibleProjectLocator
{
    IEnumerable<IExtensibleProject> GetAllExtensibleProjects(bool onlyEnabled = true);

    void Register(IExtensibleProject project);

    void Register<T>() where T : IExtensibleProject;

    void Register<T>(params object[] parameters) where T : IExtensibleProject;

    void Enable(IExtensibleProject extensibleProject);

    void Disable(IExtensibleProject extensibleProject);

    bool IsEnabled(IExtensibleProject extensibleProject);

    bool IsConfigLoaded { get; }

    void PersistChanges();

    void RestoreStateFromConfig();
}