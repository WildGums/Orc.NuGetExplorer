namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using Catel.IO;
    using Catel.Logging;
    using NuGet.Configuration;

    internal class NuGetSettings : SettingsBase<AddItem>, IVersionedSettings
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const string ConfigurationFileName = "configuration.xml";

        private readonly IConfigurationService _configurationService;

        public NuGetSettings(IConfigurationService configurationService)
            : base(configurationService)
        {
            _configurationService = configurationService;
        }

        #region ISettings

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

            var subsections = keys.Select(key =>
            {
                var value = GetValue(sectionName, key);
                return InitializeValue(sectionName, key, value);
            }).ToList();

            var section = new NuGetSettingsSection(sectionName, subsections);
            return section;
        }

        public void AddOrUpdate(string sectionName, SettingItem item)
        {
            Argument.IsNotNullOrWhitespace(() => sectionName);

            EnsureSectionExists(sectionName);

            var section = GetSection(sectionName);

            if (item is AddItem addItem)
            {

                // Hack, because we need to refresh section list of subkeys
                SetValues(sectionName, new List<AddItem> { addItem });
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
            // Note: Implementations of ISettings designed assuming that all updates are storing in-memory and flushed to disk file only on call of SaveToDisk()
            // Here we are using Catel's configuration and saving all changes instantly, thats why implementation of this method is empty
            Log.Debug("SaveToDisk method called from PackageSourceProvider");
        }

        public IList<string> GetConfigFilePaths()
        {
            var localFolderConfig = Path.Combine(DefaultNuGetFolders.GetApplicationLocalFolder(), ConfigurationFileName);
            var roamingFolderConfig = Path.Combine(DefaultNuGetFolders.GetApplicationRoamingFolder(), ConfigurationFileName);

            return new string[]
            {
                localFolderConfig,
                roamingFolderConfig
            };
        }

        public IList<string> GetConfigRoots()
        {
            return new string[]
            {
                DefaultNuGetFolders.GetApplicationLocalFolder(),
                DefaultNuGetFolders.GetApplicationRoamingFolder()
            };
        }

        #endregion

        #region SettingsBase

        protected override IList<AddItem> GetValues(string section)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            var settingsSection = GetSection(section);

            return settingsSection.Items.OfType<AddItem>().ToList();
        }

        protected override IList<AddItem> GetValues(string section, string subsection)
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

            return keys.Select(key =>
            {
                var value = GetValue(section, subsection, key);
                return InitializeValue(section, key, value);
            }
            ).ToList();
        }

        protected override void SetValues(string section, IList<AddItem> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            EnsureSectionExists(section);

            var valuesListKey = GetSectionValuesListKey(section);
            UpdateKeyList(values, valuesListKey);

            foreach (var item in values)
            {
                SetValue(section, item.Key, item.Value);
            }
        }

        protected override void SetValues(string section, string subsection, IList<AddItem> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);

            EnsureSectionExists(section);

            var valuesListKey = GetSubsectionValuesListKey(section, subsection);
            UpdateKeyList(values, valuesListKey);
            foreach (var keyValuePair in values)
            {
                SetValue(section, subsection, keyValuePair.Key, keyValuePair.Value);
            }
        }

        protected override AddItem InitializeValue(string section, string key, string value)
        {
            if (string.Equals(section, ConfigurationConstants.PackageSources) || string.Equals(section, ConfigurationConstants.DisabledPackageSources))
            {
                return new SourceItem(key, value);
            }

            return InitializeValue(key, value);
        }

        protected override AddItem InitializeValue(string key, string value)
        {
            return new AddItem(key, value);
        }

        #endregion

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

        public void UpdatePackageSourcesKeyListSorting(List<string> packageSourceNames)
        {
            var packageSourcesKeyListKey = GetSectionValuesListKey(ConfigurationConstants.PackageSources);
            var enabledPackageSourcesKeys = _configurationService.GetRoamingValue<string>(packageSourcesKeyListKey).Split(Separator);
            var sortedKeys = enabledPackageSourcesKeys.OrderBy(key => packageSourceNames.IndexOf(key));
            var sortedKeysStringValue = string.Join(Separator, sortedKeys);
            _configurationService.SetRoamingValue(packageSourcesKeyListKey, sortedKeysStringValue);
        }
    }
}
