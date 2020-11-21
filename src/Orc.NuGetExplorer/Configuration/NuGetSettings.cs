namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Catel;
    using Catel.Configuration;
    using Catel.IO;
    using Catel.Logging;
    using NuGet.Configuration;

    internal class NuGetSettings : IVersionedSettings
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

        private const char Separator = '|';
        private const string SectionListKey = "NuGet_sections";
        private const string VersionKey = "NuGetExplorer.Version";
        private const string MinimalVersionKey = "NuGetExplorer.MinimalVersion";
        private const string MinimalVersionNumber = "4.0.0";
        private const string ConfigurationFileName = "configuration.xml";

        private readonly IConfigurationService _configurationService;
        #endregion

        #region Constructors
        public NuGetSettings(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;

            //version of configuration is a version of assembly
            //get version from configuration
            GetVersionFromConfiguration();

            SettingsChanged += OnSettingsChanged;
        }
        #endregion

        public bool IsLastVersion => AssemblyVersion.Equals(Version);

        public Version Version { get; private set; }

        public Version MinimalVersion { get; private set; }

        public event EventHandler SettingsChanged;

        public event EventHandler SettingsRead;

        private void RaiseSettingsChanged()
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseSettingsRead()
        {
            SettingsRead?.Invoke(this, EventArgs.Empty);
        }

        #region ISettings

        public string GetValue(string section, string key, bool isPath = false)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var settingValue = GetNuGetValues(section, isPath).FirstOrDefault(x => string.Equals(x.Key, key));

            var result = settingValue == null ? string.Empty : settingValue.Value;

            RaiseSettingsRead();

            return result;
        }

        public IReadOnlyList<string> GetAllSubsections(string section)
        {
            RaiseSettingsRead();

            return GetNuGetValues(section).Select(subsection => subsection.Key).ToList();
        }

        public IList<KeyValuePair<string, string>> GetNestedValues(string section, string subSection)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subSection);

            RaiseSettingsRead();

            //extract key-value pairs from AddItem
            return GetNuGetValues(section, subSection)
                .Select(item => new KeyValuePair<string, string>(item.Key, item.Value))
                .ToList();
        }

        public void SetValue(string section, string key, string value)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            SetNuGetValues(section, new[] { new AddItem(key, value) });

            RaiseSettingsChanged();
        }

        public void SetNestedValues(string section, string subsection, IList<KeyValuePair<string, string>> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);

            var addItems = values.Select(x => new AddItem(x.Key, x.Value)).ToList();
            SetNuGetValues(section, subsection, addItems);

            RaiseSettingsChanged();
        }

        public bool DeleteValue(string section, string key)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            try
            {
                var valuesListKey = GetSectionValuesListKey(section);
                var keysString = _configurationService.GetRoamingValue<string>(valuesListKey);
                if (string.IsNullOrEmpty(keysString))
                {
                    return true;
                }

                var newKeys = keysString.Split(Separator).Where(x => !string.Equals(x, key));
                keysString = string.Join(Separator.ToString(), newKeys);
                _configurationService.SetRoamingValue(valuesListKey, keysString);

                var valueKey = GetSectionValueKey(section, key);
                _configurationService.SetRoamingValue(valueKey, string.Empty);
            }
            catch
            {
                return false;
            }

            RaiseSettingsChanged();

            return true;
        }

        public bool DeleteSection(string section)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            var result = true;

            try
            {
                var sectionsString = _configurationService.GetRoamingValue<string>(SectionListKey);
                if (string.IsNullOrEmpty(sectionsString))
                {
                    return true;
                }

                var newSections = sectionsString.Split(Separator).Where(x => !string.Equals(x, section));
                sectionsString = string.Join(Separator.ToString(), newSections);
                _configurationService.SetRoamingValue(SectionListKey, sectionsString);

                var values = GetNuGetValues(section, false);
                if (values == null)
                {
                    return false;
                }

                foreach (var settingValue in values)
                {
                    result = result && DeleteValue(section, settingValue.Key);
                }
            }
            catch
            {
                return false;
            }

            RaiseSettingsChanged();

            return result;
        }

        public SettingSection GetSection(string sectionName)
        {
            Argument.IsNotNullOrWhitespace(() => sectionName);

            var valuesListKey = GetSectionValuesListKey(sectionName);
            var valueKeysString = _configurationService.GetRoamingValue<string>(valuesListKey);

            if (string.IsNullOrEmpty(valueKeysString))
            {
                return new NuGetSettingsSection(sectionName);
            }

            var keys = valueKeysString.Split(Separator);

            var subsections = keys.Select(key => GetNuGetValue(sectionName, key, false)).ToList();

            RaiseSettingsRead();

            return new NuGetSettingsSection(sectionName, subsections);
        }

        public void AddOrUpdate(string sectionName, SettingItem item)
        {
            Argument.IsNotNullOrWhitespace(() => sectionName);

            EnsureSectionExists(sectionName);

            var section = GetSection(sectionName);

            if (item is AddItem addItem)
            {
                SetValue(sectionName, addItem.Key, addItem.Value);
                return;
            }

            Log.Debug($"Cannot add or update unknown item of type {item.GetType()}");
        }

        public void Remove(string sectionName, SettingItem item)
        {
            if (item is AddItem addItem)
            {
                DeleteValue(sectionName, addItem.Key);
                return;
            }

            Log.Debug($"Cannot remove unknown item of type {item.GetType()}");
        }

        public void SaveToDisk()
        {
            //should flush in-memory updates in file, but currently all changes saved manually instant in configuration file via Catel Configuration
            Log.Debug("SaveToDisk method called from PackageSourceProvider");
        }

        public IList<string> GetConfigFilePaths()
        {
            var localFolderConfig = Path.Combine(DefaultNuGetFolders.GetApplicationLocalFolder(), ConfigurationFileName);
            var roamingFolderConfig = Path.Combine(DefaultNuGetFolders.GetApplicationRoamingFolder(), ConfigurationFileName);

            return new string[] { localFolderConfig, roamingFolderConfig };
        }

        public IList<string> GetConfigRoots()
        {
            return new string[] { DefaultNuGetFolders.GetApplicationLocalFolder(), DefaultNuGetFolders.GetApplicationRoamingFolder() };
        }

        #endregion

        #region Methods

        private void SetNuGetValues(string section, IList<AddItem> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            EnsureSectionExists(section);

            var valuesListKey = GetSectionValuesListKey(section);
            UpdateKeysList(values, valuesListKey);

            foreach (var item in values)
            {
                SetNuGetValue(section, item.Key, item.Value);
            }
        }

        private void EnsureSectionExists(string section)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            var sectionsString = _configurationService.GetRoamingValue(SectionListKey, string.Empty);
            var sections = sectionsString.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!sections.Contains(section))
            {
                sections.Add(section);
                sectionsString = string.Join(Separator.ToString(), sections);
                _configurationService.SetRoamingValue(SectionListKey, sectionsString);
            }
        }


        private void SetNuGetValues(string section, string subsection, IList<AddItem> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);

            EnsureSectionExists(section);

            var valuesListKey = GetSubsectionValuesListKey(section, subsection);
            UpdateKeysList(values, valuesListKey);
            foreach (var keyValuePair in values)
            {
                SetNuGetValue(section, subsection, keyValuePair.Key, keyValuePair.Value);
            }
        }

        private void UpdateKeysList(IList<AddItem> values, string valuesListKey)
        {
            var valueKeysString = _configurationService.GetRoamingValue<string>(valuesListKey);
            var existedKeys = string.IsNullOrEmpty(valueKeysString) ? Enumerable.Empty<string>() : valueKeysString.Split(Separator);
            var keysToSave = values.Select(x => x.Key);

            var newValueKeysString = string.Join(Separator.ToString(), existedKeys.Union(keysToSave));
            _configurationService.SetRoamingValue(valuesListKey, newValueKeysString);
        }

        private string ConvertToFullPath(string result)
        {
            return result;
        }

        private IList<AddItem> GetNuGetValues(string sectionName, bool isPath = false)
        {
            Argument.IsNotNullOrWhitespace(() => sectionName);

            var section = GetSection(sectionName);

            return section.Items.OfType<AddItem>().ToList();
        }

        private IList<AddItem> GetNuGetValues(string section, string subsection, bool isPath = false)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);

            var valuesListKey = GetSubsectionValuesListKey(section, subsection);
            var valueKeysString = _configurationService.GetRoamingValue<string>(valuesListKey);
            if (string.IsNullOrEmpty(valueKeysString))
            {
                return new List<AddItem>();
            }

            var keys = valueKeysString.Split(Separator);

            return keys.Select(key => GetNuGetValue(section, subsection, key, isPath)).ToList();
        }

        private AddItem GetNuGetValue(string section, string key, bool isPath)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSectionValueKey(section, key);
            var value = _configurationService.GetRoamingValue<string>(combinedKey);

            if (isPath)
            {
                value = ConvertToFullPath(value);
            }

            if (IsSourceItem(section))
            {
                return new SourceItem(key, value);
            }

            return new AddItem(key, value);
        }

        private AddItem GetNuGetValue(string section, string subsection, string key, bool isPath)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSubsectionValueKey(section, subsection, key);
            var value = _configurationService.GetRoamingValue<string>(combinedKey);

            if (isPath)
            {
                value = ConvertToFullPath(value);
            }

            if (IsSourceItem(section))
            {
                return new SourceItem(key, value);
            }

            return new AddItem(key, value);
        }

        private void SetNuGetValue(string section, string key, string value)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSectionValueKey(section, key);
            _configurationService.SetRoamingValue(combinedKey, value);
        }

        private void SetNuGetValue(string section, string subsection, string key, string value)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSubsectionValueKey(section, subsection, key);
            _configurationService.SetRoamingValue(combinedKey, value);
        }

        private void GetVersionFromConfiguration()
        {
            var configurationVersionString = _configurationService.GetRoamingValue<string>(VersionKey);

            Version configurationVersion = null;

            if (!string.IsNullOrEmpty(configurationVersionString) && Version.TryParse(configurationVersionString, out configurationVersion))
            {
                Version = configurationVersion;
            }

            var configurationMinimalVersionString = _configurationService.GetRoamingValue<string>(MinimalVersionKey);

            if (!string.IsNullOrEmpty(configurationMinimalVersionString) && Version.TryParse(configurationMinimalVersionString, out configurationVersion))
            {
                MinimalVersion = configurationVersion;
            }

            RaiseSettingsRead();
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            SettingsChanged -= OnSettingsChanged;
            //write version one time
            UpdateMinimalVersion();
        }

        private string GetSectionValueKey(string section, string key)
        {
            return $"NuGet_{section}_value_{key}";
        }

        private string GetSubsectionValueKey(string section, string subsection, string key)
        {
            return $"NuGet_{section}_{subsection}_value_{key}";
        }

        private static string GetSectionValuesListKey(string section)
        {
            return $"NuGet_{section}_values";
        }

        private static string GetSubsectionValuesListKey(string section, string subsection)
        {
            return $"NuGet_{section}_{subsection}_values";
        }

        private static bool IsSourceItem(string sectionKey)
        {
            return string.Equals(sectionKey, ConfigurationConstants.PackageSources) || string.Equals(sectionKey, ConfigurationConstants.DisabledPackageSources);
        }

        public void UpdateVersion()
        {
            _configurationService.SetRoamingValue(VersionKey, AssemblyVersion);
        }

        public void UpdateMinimalVersion()
        {
            _configurationService.SetRoamingValue(MinimalVersionKey, MinimalVersionNumber);
        }

        #endregion
    }
}
