namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Catel;
    using Catel.Configuration;
    using NuGet.Configuration;

    public abstract class SettingsBase
    {
        protected static readonly Version _assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

        protected const string MinimalVersionKey = "NuGetExplorer.MinimalVersion";
        protected const string VersionKey = "NuGetExplorer.Version";

        protected const char Separator = '|';
        protected const string SectionListKey = "NuGet_sections";

        protected const string MinimalVersionNumber = "4.0.0";
    }

    public abstract class SettingsBase<T> : SettingsBase where T : AddItem
    {
        private readonly IConfigurationService _configurationService;

        protected SettingsBase(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);
            _configurationService = configurationService;

            //version of configuration is a version of assembly
            //get version from configuration
            GetVersionsFromConfiguration();

            SettingsChanged += OnSettingsChanged;
        }

        public event EventHandler SettingsChanged;

        protected void RaiseSettingsChanged()
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<SettingsReadEventArgs> SettingsRead;

        protected void RaiseSettingsRead(string key)
        {
            SettingsRead?.Invoke(this, new SettingsReadEventArgs(key));
        }

        public bool IsLastVersion => _assemblyVersion.Equals(Version);

        public Version Version { get; protected set; }

        public Version MinimalVersion { get; protected set; }

        public string GetValue(string section, string key)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSectionValueKey(section, key);
            var value = _configurationService.GetRoamingValue<string>(combinedKey, string.Empty);

            RaiseSettingsRead(combinedKey);

            return value;
        }

        public string GetValue(string section, string subsection, string key)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSubsectionValueKey(section, subsection, key);
            var value = _configurationService.GetRoamingValue<string>(combinedKey);

            RaiseSettingsRead(combinedKey);

            return value;
        }

        public void SetValue(string section, string key, string value)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSectionValueKey(section, key);
            _configurationService.SetRoamingValue(combinedKey, value);

            RaiseSettingsChanged();
        }

        public void SetValue(string section, string subsection, string key, string value)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSubsectionValueKey(section, subsection, key);
            _configurationService.SetRoamingValue(combinedKey, value);

            RaiseSettingsChanged();
        }

        public IReadOnlyList<string> GetAllSubsections(string section)
        {
            return GetValues(section).Select(subsection => subsection.Key).ToList();
        }

        public IList<KeyValuePair<string, string>> GetNestedValues(string section, string subSection)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subSection);

            //extract key-value pairs from AddItem
            return GetValues(section, subSection)
                .Select(item => new KeyValuePair<string, string>(item.Key, item.Value))
                .ToList();
        }

        public void SetNestedValues(string section, string subsection, IList<KeyValuePair<string, string>> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);

            var addItems = values.Select(x => InitializeValue(x.Key, x.Value)).ToList();
            SetValues(section, subsection, addItems);
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

                RaiseSettingsChanged();
            }
            catch
            {
                return false;
            }

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

                RaiseSettingsChanged();

                var values = GetValues(section);
                if (values is null)
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

            return result;
        }

        public void UpdateVersion()
        {
            _configurationService.SetRoamingValue(VersionKey, _assemblyVersion);
        }

        public void UpdateMinimalVersion()
        {
            _configurationService.SetRoamingValue(MinimalVersionKey, MinimalVersionNumber);
        }

        protected abstract IList<T> GetValues(string section);
        protected abstract IList<T> GetValues(string section, string subsection);
        protected abstract void SetValues(string section, IList<T> values);
        protected abstract void SetValues(string section, string subsection, IList<T> values);
        protected abstract T InitializeValue(string key, string value);
        protected abstract T InitializeValue(string section, string key, string value);

        protected void UpdateKeyList(IList<AddItem> values, string valuesListKey)
        {
            var valueKeysString = _configurationService.GetRoamingValue<string>(valuesListKey);
            var existedKeys = string.IsNullOrEmpty(valueKeysString) ? Enumerable.Empty<string>() : valueKeysString.Split(Separator);
            var keysToSave = values.Select(x => x.Key);

            var newValueKeysString = string.Join(Separator.ToString(), existedKeys.Union(keysToSave));
            _configurationService.SetRoamingValue(valuesListKey, newValueKeysString);
        }

        /// <summary>
        /// Updates minimal version in the config one time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSettingsChanged(object sender, EventArgs e)
        {
            SettingsChanged -= OnSettingsChanged;
            UpdateMinimalVersion();
        }

        protected virtual string GetSectionValueKey(string section, string key)
        {
            return $"NuGet_{section}_value_{key}";
        }

        protected virtual string GetSectionValuesListKey(string section)
        {
            return $"NuGet_{section}_values";
        }

        protected virtual string GetSubsectionValueKey(string section, string subsection, string key)
        {
            return $"NuGet_{section}_{subsection}_value_{key}";
        }

        protected virtual string GetSubsectionValuesListKey(string section, string subsection)
        {
            return $"NuGet_{section}_{subsection}_values";
        }

        private void GetVersionsFromConfiguration()
        {
            var configurationVersionString = _configurationService.GetRoamingValue<string>(VersionKey);

            Version configurationVersion;
            if (!string.IsNullOrEmpty(configurationVersionString) && Version.TryParse(configurationVersionString, out configurationVersion))
            {
                Version = configurationVersion;
            }

            RaiseSettingsRead(configurationVersionString);

            var configurationMinimalVersionString = _configurationService.GetRoamingValue<string>(MinimalVersionKey);

            if (!string.IsNullOrEmpty(configurationMinimalVersionString) && Version.TryParse(configurationMinimalVersionString, out configurationVersion))
            {
                MinimalVersion = configurationVersion;
            }

            RaiseSettingsRead(configurationMinimalVersionString);
        }
    }
}
