namespace Orc.NuGetExplorer.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using NuGetExplorer.Services;

    public class ExtensibleProjectLocator : IExtensibleProjectLocator
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITypeFactory _typeFactory;

        private readonly NugetConfigurationService _managerConfigurationService;

        private readonly Dictionary<Type, IExtensibleProject> _registredProjects = new Dictionary<Type, IExtensibleProject>();

        private readonly HashSet<IExtensibleProject> _enabledProjects = new HashSet<IExtensibleProject>();

        public ExtensibleProjectLocator(ITypeFactory typeFactory, IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => configurationService);

            _typeFactory = typeFactory;
            _managerConfigurationService = configurationService as NugetConfigurationService;

            Argument.IsNotNull(() => _managerConfigurationService);
        }

        public bool IsConfigLoaded { get; private set; }

        public bool IsEnabled(IExtensibleProject extensibleProject)
        {
            return _enabledProjects.Contains(extensibleProject);
        }

        public void Enable(IExtensibleProject extensibleProject)
        {
            var registeredProject = _registredProjects[extensibleProject.GetType()];

            if (registeredProject != extensibleProject)
            {
                throw new ProjectStateException("ExtensibleProject must be registered before use");
            }

            if (!_enabledProjects.Add(registeredProject))
            {
                Log.Info($"Project {extensibleProject} already enabled");
            }
        }

        public void Disable(IExtensibleProject extensibleProject)
        {
            var registeredProject = _registredProjects[extensibleProject.GetType()];

            if (registeredProject != extensibleProject)
            {
                throw new InvalidOperationException("ExtensibleProject must be registered before use");
            }

            if (!_enabledProjects.Remove(registeredProject))
            {
                Log.Info($"Attempt to disable Project {extensibleProject}, which is not enabled");
            }
        }

        public IEnumerable<IExtensibleProject> GetAllExtensibleProjects(bool onlyEnabled = true)
        {
            if(onlyEnabled)
            {
                return _enabledProjects.ToList();
            }

            return _registredProjects.Values.ToList();
        }

        public void Register(IExtensibleProject project)
        {
            _registredProjects[project.GetType()] = project;
        }

        public void Register<T>() where T : IExtensibleProject
        {
            Register<T>(new object[] { });
        }

        public void Register<T>(params object[] parameters) where T : IExtensibleProject
        {
            var instance = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<T>(parameters);

            Register(instance);
        }

        public void PersistChanges()
        {
            _managerConfigurationService.SetRoamingValueWithDefaultIdGenerator(
                _enabledProjects.Select(x =>
                    x.GetType().FullName)
                .ToList()
                );
        }

        public void RestoreStateFromConfig()
        {
            try
            {
                var restored = _managerConfigurationService.GetRoamingValue(Configuration.ConfigurationSections.ProjectExtensions) as List<string>;

                foreach (var type in _registredProjects.Keys.Where(x => restored.Any(s => s == x.FullName)))
                {
                    Enable(_registredProjects[type]);
                }
            }
            catch (ProjectStateException e)
            {
                Log.Error(e, "Mismatch between configuration and registered projects");
            }
            catch (Exception e)
            {
                Log.Error(e, "Error when restoring project extensions state from configuration");
            }
            finally
            {
                IsConfigLoaded = true;
            }
        }
    }
}
