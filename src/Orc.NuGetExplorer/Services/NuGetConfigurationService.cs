namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Services;
    using NuGet.Configuration;
    using NuGetExplorer.Configuration;
    using Settings = Settings;

    public class NuGetConfigurationService : INuGetConfigurationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;
        private readonly string _defaultDestinationFolder;
        private int _packageQuerySize = 40;

        // Note: Lazy allows to avoid circular types resolving if construct from TypeFactory
        private readonly Lazy<IPackageSourceProvider> _packageSourceProvider = new(
                () =>
                {
                    return ServiceLocator.Default.ResolveRequiredType<IPackageSourceProvider>();
                }
            );

        private readonly Dictionary<ConfigurationSection, string> _masterKeys = new()
        {
            { ConfigurationSection.Feeds, $"NuGet_{ConfigurationSection.Feeds}" },
            { ConfigurationSection.ProjectExtensions, $"NuGet_{ConfigurationSection.ProjectExtensions}" }
        };

        public NuGetConfigurationService(IConfigurationService configurationService, IAppDataService appDataService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;

            _defaultDestinationFolder = Path.Combine(appDataService.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming), "plugins");
        }


        #region INuGetConfigurationService

        public string GetDestinationFolder()
        {
            var destinationFolder = _configurationService.GetRoamingValue(Settings.NuGet.DestinationFolder, _defaultDestinationFolder);
            return string.IsNullOrEmpty(destinationFolder) ? _defaultDestinationFolder : destinationFolder;
        }

        public void SetDestinationFolder(string value)
        {
            Argument.IsNotNullOrWhitespace(() => value);

            _configurationService.SetRoamingValue(Settings.NuGet.DestinationFolder, value);
        }

        public IEnumerable<IPackageSource> LoadPackageSources(bool onlyEnabled = false)
        {
            var packageSources = _packageSourceProvider.Value.LoadPackageSources();

            if (onlyEnabled)
            {
                packageSources = packageSources.Where(x => x.IsEnabled);
            }

            return packageSources.ToPackageSourceInterfaces();
        }

        public bool SavePackageSource(string name, string source, bool isEnabled = true, bool isOfficial = true, bool verifyFeed = true)
        {
            Argument.IsNotNullOrWhitespace(() => name);
            Argument.IsNotNullOrWhitespace(() => source);

            try
            {
                var packageSources = _packageSourceProvider.Value.LoadPackageSources().ToList();

                var existedSource = packageSources.FirstOrDefault(x => string.Equals(x.Name, name));

                if (existedSource is null)
                {
                    existedSource = new PackageSource(source, name);
                    packageSources.Add(existedSource);
                }

                existedSource.IsEnabled = isEnabled;
                existedSource.IsOfficial = isOfficial;

                _packageSourceProvider.Value.SavePackageSources(packageSources);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void DisablePackageSource(string name, string source)
        {
            Argument.IsNotNullOrWhitespace(() => name);

            _packageSourceProvider.Value.DisablePackageSource(name);
        }

        public void RemovePackageSource(IPackageSource source)
        {
            _packageSourceProvider.Value.RemovePackageSource(source.Name);
        }

        public void SavePackageSources(IEnumerable<IPackageSource> packageSources)
        {
            Argument.IsNotNull(() => packageSources);
            _packageSourceProvider.Value.SavePackageSources(packageSources.ToPackageSourceInstances());

            if (_packageSourceProvider.Value is NuGetPackageSourceProvider nugetPackageSourceProvider)
            {
                nugetPackageSourceProvider.SortPackageSources(packageSources.Select(x => x.Name).ToList());
            }
        }

        public void SetIsPrereleaseAllowed(IRepository repository, bool value)
        {
            Argument.IsNotNull(() => repository);

            var key = GetIsPrereleaseAllowedKey(repository);
            _configurationService.SetRoamingValue(key, value);
        }

        public bool GetIsPrereleaseAllowed(IRepository repository)
        {
            var key = GetIsPrereleaseAllowedKey(repository);
            var stringValue = _configurationService.GetRoamingValue(key, false.ToString());

            if (bool.TryParse(stringValue, out var value))
            {
                return value;
            }

            return false;
        }

        private string GetIsPrereleaseAllowedKey(IRepository repository)
        {
            return string.Format("NuGetExplorer.IsPrereleaseAllowed.{0}", repository.OperationType);
        }

        public void SaveProjects(IEnumerable<IExtensibleProject> extensibleProjects)
        {
            foreach (var project in extensibleProjects)
            {
                var key = GetProjectKey(project);
                _configurationService.SetRoamingValue(key, project);
            }
        }

        public bool IsProjectConfigured(IExtensibleProject project)
        {
            return _configurationService.GetRoamingValue<bool>(GetProjectKey(project));
        }

        #endregion

        private string GetProjectKey(IExtensibleProject extensibleProject)
        {
            return $"{_masterKeys[ConfigurationSection.ProjectExtensions]}_{extensibleProject.GetType().FullName}";
        }

        public void SetPackageQuerySize(int size)
        {
            _packageQuerySize = size;
        }

        public int GetPackageQuerySize()
        {
            return _packageQuerySize;
        }
    }
}
