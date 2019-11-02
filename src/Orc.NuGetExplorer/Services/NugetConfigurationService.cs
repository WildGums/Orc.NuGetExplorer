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
    using Settings = Orc.NuGetExplorer.Settings;

    public class NugetConfigurationService : ConfigurationService, INuGetConfigurationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IXmlSerializer _configSerializer;
        private readonly string _defaultDestinationFolder;

        //had to doing this, because settings is as parameter in ctor caused loop references
        private readonly Lazy<Configuration.IPackageSourceProvider> _packageSourceProvider = new Lazy<IPackageSourceProvider>(
                () => { 
                    return ServiceLocator.Default.ResolveType<Configuration.IPackageSourceProvider>(); 
                }
            );

        private readonly Dictionary<ConfigurationSections, string> _masterKeys = new Dictionary<ConfigurationSections, string>()
        {
            { ConfigurationSections.Feeds, $"NuGet_{ConfigurationSections.Feeds}" },
            { ConfigurationSections.ProjectExtensions, $"NuGet_{ConfigurationSections.ProjectExtensions}" }
        };


        public NugetConfigurationService(ISerializationManager serializationManager,
            IObjectConverterService objectConverterService, IXmlSerializer serializer) : base(serializationManager, objectConverterService, serializer)
        {
            _configSerializer = serializer;

            _defaultDestinationFolder = Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "plugins");
        }


        #region INuGetConfigurationService
        public string GetDestinationFolder()
        {
            return this.GetRoamingValue(Settings.NuGet.DestinationFolder, _defaultDestinationFolder);
        }

        public void SetDestinationFolder(string value)
        {
            Argument.IsNotNullOrWhitespace(() => value);

            this.SetRoamingValue(Settings.NuGet.DestinationFolder, value);
        }

        public IEnumerable<IPackageSource> LoadPackageSources(bool onlyEnabled = false)
        {
            //todo implement packageSourceProvider
            //var packageSources = PackageSourceProvider.LoadPackageSources();

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
            throw new NotImplementedException();
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
            this.SetRoamingValue(key, value);
        }

        public bool GetIsPrereleaseAllowed(IRepository repository)
        {
            var key = GetIsPrereleaseAllowedKey(repository);
            var stringValue = this.GetRoamingValue(key, false.ToString());

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

        #endregion

        public NuGetFeed GetValue(ConfigurationContainer container, string key)
        {
            var rawXml = GetValueFromStore(container, key);

            var ser = new System.Xml.Serialization.XmlSerializer(typeof(NuGetFeed));

            object serializedModel = null;

            using (StringReader sr = new StringReader(rawXml))
            {
                serializedModel = ser.Deserialize(sr);
            }

            if (serializedModel == null)
            {
                return null;
            }
            else
            {
                var serializedFeed = serializedModel as NuGetFeed;

                Guid guid = KeyFromString(key);

                if (serializedFeed != null)
                {
                    if (!ConfigurationIdGenerator.TryTakeUniqueIdentifier(guid, out Guid newGUid))
                    {
                        serializedFeed.SerializationIdentifier = newGUid;
                    }
                    else
                    {
                        serializedFeed.SerializationIdentifier = guid;
                    }

                    serializedFeed.Initialize();

                    return serializedFeed;
                }
                throw new InvalidCastException("Serialized model cannot be casted to type 'NuGetFeed'");
            }
        }

        public object GetRoamingValue(ConfigurationSections section)
        {
            var masterKey = _masterKeys[section];

            var obj = DeserializeXmlToListOfString(ConfigurationContainer.Roaming, masterKey);

            return obj;
        }

        public NuGetFeed GetRoamingValue(Guid key)
        {
            return GetValue(ConfigurationContainer.Roaming, KeyToString(key));
        }

        public IReadOnlyList<Guid> GetAllKeys(ConfigurationContainer container)
        {
            var feedKeys = GetValueFromStore(container, _masterKeys[ConfigurationSections.Feeds]);

            var keyList = feedKeys.Split(new string[] { Constants.ConfigKeySeparator }, StringSplitOptions.RemoveEmptyEntries);

            return keyList.Select(key => KeyFromString(key)).ToList();
        }

        public void SetValue(ConfigurationContainer container, string key, NuGetFeed value)
        {
            using (var memstream = new MemoryStream())
            {
                var rawxml = SerializeXml(memstream, () => value.Save(memstream, _configSerializer));

                bool shouldBeUpdated = !ValueExists(container, key);

                SetValueToStore(container, key, rawxml);

                if (shouldBeUpdated)
                {
                    UpdateSectionKeyList(container, ConfigurationSections.Feeds, key);
                }
            }
        }

        public void SetValueWithDefaultIdGenerator(ConfigurationContainer container, NuGetFeed value)
        {
            if (value.SerializationIdentifier.Equals(default(Guid)))
            {
                value.SerializationIdentifier = ConfigurationIdGenerator.GetUniqueIdentifier();
            }

            SetValue(container, KeyToString(value.SerializationIdentifier), value);
        }

        public void SetRoamingValueWithDefaultIdGenerator(NuGetFeed value)
        {
            SetValueWithDefaultIdGenerator(ConfigurationContainer.Roaming, value);
        }

        public void SetRoamingValueWithDefaultIdGenerator(List<string> extensibleProject)
        {
            SetValueInSection(ConfigurationContainer.Roaming, ConfigurationSections.ProjectExtensions, extensibleProject);
        }

        public void RemoveValues(ConfigurationContainer container, IReadOnlyList<NuGetFeed> feedList)
        {
            foreach (var feed in feedList)
            {
                var guid = feed.SerializationIdentifier;

                if (!guid.Equals(default(Guid)))
                {
                    guid = ConfigurationIdGenerator.IsCollision(guid) ? ConfigurationIdGenerator.GetOriginalIdentifier(guid) : guid;

                    if (IsValueAvailable(container, KeyToString(guid)))
                    {
                        SetValue(container, KeyToString(guid), String.Empty);

                        UpdateSectionKeyList(container, ConfigurationSections.Feeds, KeyToString(guid), true);
                    }
                }
            }
        }

        protected string KeyToString(Guid guid)
        {
            return $"_{guid}";
        }

        protected Guid KeyFromString(string key)
        {
            return Guid.Parse(key.Substring(1));
        }

        private string SerializeXml(Stream stream, Action putValueToStream)
        {
            putValueToStream();

            var streamReader = new StreamReader(stream);

            stream.Position = 0;

            string rawxml = streamReader.ReadToEnd();

            return rawxml;
        }

        private object DeserializeXmlToListOfString(ConfigurationContainer container, string key)
        {
            var storedValue = GetValueFromStore(container, key);

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

        private void SetValueInSection(ConfigurationContainer container, ConfigurationSections section, object value)
        {
            using (var memStream = new MemoryStream())
            {
                var strValue = SerializeXml(memStream, () => _configSerializer.Serialize(value, memStream));

                SetValueToStore(container, _masterKeys[section], strValue);
            }
        }

        private void UpdateSectionKeyList(ConfigurationContainer container, ConfigurationSections confSection, string key, bool isRemove = false)
        {
            var keyList = GetValueFromStore(container, _masterKeys[confSection]);
            string updatedKeys = String.Empty;

            if (isRemove)
            {
                var persistedKeys = keyList.Split(new string[] { Constants.ConfigKeySeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //updatedKeys = String.Join(KeySeparator, persistedKeys.Except(keys));
                persistedKeys.Remove(key);
                updatedKeys = String.Join(Constants.ConfigKeySeparator, persistedKeys);
            }
            else
            {
                //updatedKeys = String.Join(keyList, keys);
                updatedKeys = String.Join(Constants.ConfigKeySeparator, key, keyList);
            }

            SetValueToStore(container, _masterKeys[confSection], updatedKeys);
        }

        [XmlRoot(ElementName = "Items")]
        public class ListWrapper
        {
            [XmlElement(ElementName = "string", Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
            public List<string> List { get; set; }
        }
    }
}
