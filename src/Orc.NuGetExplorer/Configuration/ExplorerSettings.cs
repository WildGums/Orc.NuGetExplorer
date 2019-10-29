namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Catel;
    using Catel.Configuration;
    using NuGet.Configuration;

    public class ExplorerSettings : ISettings
    {
        private readonly IConfigurationService _configurationService;

        private readonly Dictionary<ConfigurationSections, string> _masterKeys = new Dictionary<ConfigurationSections, string>()
        {
            { ConfigurationSections.Feeds, $"NuGet_{ConfigurationSections.Feeds}" },
            { ConfigurationSections.ProjectExtensions, $"NuGet_{ConfigurationSections.ProjectExtensions}" }
        };

        public ExplorerSettings(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            //todo
            _configurationService = configurationService;
        }

        public event EventHandler SettingsChanged;

        public void AddOrUpdate(string sectionName, SettingItem item)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSection(string section)
        {
            throw new NotImplementedException();
        }

        public bool DeleteValue(string section, string key)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<string> GetAllSubsections(string section)
        {
            throw new NotImplementedException();
        }

        public IList<string> GetConfigFilePaths()
        {
            throw new NotImplementedException();
        }

        public IList<string> GetConfigRoots()
        {
            return _masterKeys.Values.Cast<string>().ToList();
        }

        [Obsolete]
        public IReadOnlyList<SettingValue> GetNestedSettingValues(string section, string subSection)
        {
            throw new NotImplementedException();
        }

        public IList<KeyValuePair<string, string>> GetNestedValues(string section, string subSection)
        {
            throw new NotImplementedException();
        }

        public SettingSection GetSection(string sectionName)
        {
            var sectionKey = _masterKeys.FirstOrDefault(x => string.Equals(sectionName, x.Key)).Value;

            if (string.IsNullOrEmpty(sectionKey))
            {
                throw new InvalidOperationException($"Section {sectionName} does not exist in configuration");
            }

            var sectionValue = DeserializeXmlToListOfString(ConfigurationContainer.Roaming, sectionKey);

            //todo 
            throw new NotImplementedException();
        }

        [Obsolete]
        public IList<SettingValue> GetSettingValues(string section, bool isPath = false)
        {
            throw new NotImplementedException();
        }

        public string GetValue(string section, string key, bool isPath = false)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var settingValue = GetValues(section, isPath).FirstOrDefault(x => string.Equals(x.Key, key));

            var result = settingValue == null ? string.Empty : settingValue.Value;

            return result;
        }

        public IList<AddItem> GetValues(string section, bool isPath)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            var valuesListKey = GetKeyFromSection(section);
            var valueKeysString = _configurationService.GetRoamingValue<string>(valuesListKey);
            if (string.IsNullOrEmpty(valueKeysString))
            {
                return new List<AddItem>();
            }

            var keys = valueKeysString.Split(new string[] { Constants.ConfigKeySeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return keys.Select(key => GetValueFromKey(section, key, isPath)).ToList();
        }

        private AddItem GetValueFromKey(string section, string key, bool isPath)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = CombineKey(section, key);
            var value = _configurationService.GetRoamingValue<string>(combinedKey);

            if (isPath)
            {
                //todo
                //value = ConvertToFullPath(value);
                throw new NotImplementedException();
            }

            return new AddItem(key, value);
        }

        public void Remove(string sectionName, SettingItem item)
        {
            throw new NotImplementedException();
        }

        public void SaveToDisk()
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public void SetNestedSettingValues(string section, string subsection, IList<SettingValue> values)
        {
            throw new NotImplementedException();
        }

        public void SetNestedValues(string section, string subsection, IList<KeyValuePair<string, string>> values)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string section, string key, string value)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public void SetValues(string section, IReadOnlyList<SettingValue> values)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public void UpdateSections(string section, IReadOnlyList<SettingValue> values)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public void UpdateSubsections(string section, string subsection, IReadOnlyList<SettingValue> values)
        {
            throw new NotImplementedException();
        }

        private void RaiseSettingsChanged()
        {
            //todo provide event args
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        #region configuration

        public object GetRoamingValue(ConfigurationSections section)
        {
            var masterKey = _masterKeys[section];

            var obj = DeserializeXmlToListOfString(ConfigurationContainer.Roaming, masterKey);

            return obj;
        }

        private object DeserializeXmlToListOfString(ConfigurationContainer container, string key)
        {
            var storedValue = _configurationService.GetValue<string>(container, key);

            if (string.IsNullOrEmpty(storedValue))
            {
                return new List<string>();
            }

            var ser = new XmlSerializer(typeof(ListWrapper));

            using (StringReader sr = new StringReader(storedValue))
            {
                var obj = ser.Deserialize(sr);

                return (obj as ListWrapper)?.List;
            }
        }

        private string GetKeyFromSection(string section)
        {
            var confSection = (ConfigurationSections)Enum.Parse(typeof(ConfigurationSections), section);

            return _masterKeys[confSection];
        }

        private string CombineKey(string section, string key)
        {
            return $"NuGet_{section}_value_{key}";
        }

        [XmlRoot(ElementName = "Items")]
        public class ListWrapper
        {
            [XmlElement(ElementName = "string", Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
            public List<string> List { get; set; }
        }

        #endregion
    }
}
