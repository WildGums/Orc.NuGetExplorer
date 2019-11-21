namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Catel;
    using Catel.Configuration;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Catel.Services;
    using NuGet.Configuration;
    using NuGetExplorer.Configuration;
    using NuGetExplorer.Models;
    using Configuration = NuGet.Configuration;
    using Settings = Settings;

    public class NuGetConfigurationService : INuGetConfigurationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IXmlSerializer _configSerializer;
        private readonly IConfigurationService _configurationService;
        private readonly string _defaultDestinationFolder;

        //had to doing this, because settings is as parameter in ctor caused loop references
        private readonly Lazy<IPackageSourceProvider> _packageSourceProvider = new Lazy<IPackageSourceProvider>(
                () =>
                {
                    return ServiceLocator.Default.ResolveType<IPackageSourceProvider>();
                }
            );

        private readonly Dictionary<ConfigurationSection, string> _masterKeys = new Dictionary<ConfigurationSection, string>()
        {
            { ConfigurationSection.Feeds, $"NuGet_{ConfigurationSection.Feeds}" },
            { ConfigurationSection.ProjectExtensions, $"NuGet_{ConfigurationSection.ProjectExtensions}" }
        };

        public NuGetConfigurationService(IXmlSerializer serializer, IConfigurationService configurationService, IAppDataService appDataService)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => serializer);

            _configSerializer = serializer;
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

                if (existedSource == null)
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
            //todo implement packageSource removal
        }

        public void SavePackageSources(IEnumerable<IPackageSource> packageSources)
        {
            Argument.IsNotNull(() => packageSources);
            _packageSourceProvider.Value.SavePackageSources(packageSources.ToPackageSourceInstances());
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

            bool value;
            if (bool.TryParse(stringValue, out value))
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
            SetRoamingValueWithDefaultIdGenerator(
                    extensibleProjects.Select(x =>
                        x.GetType().FullName)
                    .ToList()
               );
        }

        public object GetSectionValues(ConfigurationSection section)
        {
            return GetRoamingValue(section);
        }

        #endregion

        public object GetRoamingValue(ConfigurationSection section)
        {
            var masterKey = _masterKeys[section];

            var obj = DeserializeXmlToListOfString(ConfigurationContainer.Roaming, masterKey);

            return obj;
        }

        public void SetRoamingValueWithDefaultIdGenerator(List<string> extensibleProject)
        {
            SetValueInSection(ConfigurationContainer.Roaming, ConfigurationSection.ProjectExtensions, extensibleProject);
        }

        private object DeserializeXmlToListOfString(ConfigurationContainer container, string key)
        {
            //var storedValue = _configurationService.GetValueFromStore(container, key);
            var storedValue = _configurationService.GetValue<string>(container, key);

            if (string.IsNullOrEmpty(storedValue))
            {
                return new List<string>();
            }

            var ser = new System.Xml.Serialization.XmlSerializer(typeof(ListWrapper));

            using (StringReader sr = new StringReader(storedValue))
            {
                var obj = ser.Deserialize(sr);

                return (obj as ListWrapper)?.List;
            }
        }

        private void SetValueInSection(ConfigurationContainer container, ConfigurationSection section, object value)
        {
            using (var memStream = new MemoryStream())
            {
                var strValue = SerializeXml(memStream, () => _configSerializer.Serialize(value, memStream));

                //SetValueToStore(container, _masterKeys[section], strValue);
                _configurationService.SetValue(container, _masterKeys[section], strValue);
            }
        }

        private string SerializeXml(Stream stream, Action putValueToStream)
        {
            putValueToStream();

            var streamReader = new StreamReader(stream);

            stream.Position = 0;

            string rawxml = streamReader.ReadToEnd();

            return rawxml;
        }


        [XmlRoot(ElementName = "Items")]
        public class ListWrapper
        {
            [XmlElement(ElementName = "string", Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
            public List<string> List { get; set; }
        }

    }
}
