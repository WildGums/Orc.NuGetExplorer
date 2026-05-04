namespace Orc.NuGetExplorer.Management;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.IoC;
using Catel.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class ExtensibleProjectLocator : IExtensibleProjectLocator
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ExtensibleProjectLocator));

    private readonly IServiceProvider _serviceProvider;

    private readonly INuGetConfigurationService _managerConfigurationService;

    private readonly Dictionary<Type, IExtensibleProject> _registeredProjects = new();

    private readonly HashSet<IExtensibleProject> _enabledProjects = new();

    public ExtensibleProjectLocator(IServiceProvider serviceProvider, INuGetConfigurationService configurationService)
    {
        _serviceProvider = serviceProvider;
        _managerConfigurationService = configurationService;
    }

    public bool IsConfigLoaded { get; private set; }

    public bool IsEnabled(IExtensibleProject extensibleProject)
    {
        ArgumentNullException.ThrowIfNull(extensibleProject);

        return _enabledProjects.Contains(extensibleProject);
    }

    public void Enable(IExtensibleProject extensibleProject)
    {
        ArgumentNullException.ThrowIfNull(extensibleProject);

        var registeredProject = _registeredProjects[extensibleProject.GetType()];

        if (registeredProject != extensibleProject)
        {
            throw Logger.LogErrorAndCreateException<ProjectStateException>("ExtensibleProject must be registered before use");
        }

        if (!_enabledProjects.Add(registeredProject))
        {
            Logger.LogInformation("Project {Project} already enabled", extensibleProject);
        }
    }

    public void Disable(IExtensibleProject extensibleProject)
    {
        ArgumentNullException.ThrowIfNull(extensibleProject);

        var registeredProject = _registeredProjects[extensibleProject.GetType()];

        if (registeredProject != extensibleProject)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("ExtensibleProject must be registered before use");
        }

        if (!_enabledProjects.Remove(registeredProject))
        {
            Logger.LogInformation("Attempt to disable Project {Project}, which is not enabled", extensibleProject);
        }
    }

    public IReadOnlyList<IExtensibleProject> GetAllExtensibleProjects(bool onlyEnabled = true)
    {
        if (onlyEnabled)
        {
            return _enabledProjects.ToArray();
        }

        return _registeredProjects.Values.ToArray();
    }

    public void Register(IExtensibleProject project)
    {
        ArgumentNullException.ThrowIfNull(project);

        _registeredProjects[project.GetType()] = project;
    }

    public void Register<T>()
        where T : IExtensibleProject
    {
        var project = ActivatorUtilities.CreateInstance<T>(_serviceProvider);

        Register(project);
    }

    public void Register<T>(params object[] parameters)
        where T : IExtensibleProject
    {
        if (parameters is null)
        {
            Register<T>();
        }
        else
        {
            var project = ActivatorUtilities.CreateInstance<T>(_serviceProvider, parameters);

            Register(project);
        }
    }

    public void PersistChanges()
    {
        _managerConfigurationService.SaveProjects(_enabledProjects.ToArray());
    }

    public void RestoreStateFromConfig()
    {
        try
        {
            foreach (var project in _registeredProjects.Values)
            {
                if (_managerConfigurationService.IsProjectConfigured(project))
                {
                    Enable(project);
                }
            }
        }
        catch (ProjectStateException ex)
        {
            Logger.LogError(ex, "Mismatch between configuration and registered projects");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error when restoring project extensions state from configuration");
        }
        finally
        {
            IsConfigLoaded = true;
        }
    }
}
