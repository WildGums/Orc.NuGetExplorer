// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetSettings.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using NuGet;

    internal class NuGetSettings : ISettings
    {
        #region Fields
        private const char Separator = '|';
        private readonly IConfigurationService _configurationService;
        #endregion

        #region Constructors
        public NuGetSettings(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;
        }
        #endregion

        #region Methods
        public string GetValue(string section, string key, bool isPath)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var settingValue = GetValues(section, isPath).FirstOrDefault(x => string.Equals(x.Key, key));

            var result = settingValue == null ? string.Empty : settingValue.Value;

            return result;
        }

        public IList<SettingValue> GetValues(string section, bool isPath)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            return GetNuGetValues(section, isPath);
        }

        public IList<SettingValue> GetNestedValues(string section, string subsection)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);

            return GetNuGetValues(section, subsection);
        }

        public void SetValue(string section, string key, string value)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            SetNuGetValues(section, new[] {new KeyValuePair<string, string>(key, value)});
        }

        public void SetValues(string section, IList<SettingValue> values)
        {
            foreach (var value in values)
            {
                SetValue(section, value.Key, value.Value);
            }
        }

        public void UpdateSections(string section, IList<SettingValue> values)
        {
            foreach (var value in values)
            {
                SetValue(section, value.Key, value.Value);
            }
        }

        public void SetValues(string section, IList<KeyValuePair<string, string>> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            SetNuGetValues(section, values);
        }

        public void SetNestedValues(string section, string key, IList<KeyValuePair<string, string>> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            SetNuGetValues(section, key, values);
        }

        public bool DeleteValue(string section, string key)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            try
            {
                var valuesListKey = GetSectionValuesListKey(section);
                var keysString = _configurationService.GetValue<string>(valuesListKey);
                if (string.IsNullOrEmpty(keysString))
                {
                    return true;
                }

                var newKeys = keysString.Split(Separator).Where(x => !string.Equals(x, key));
                keysString = string.Join(Separator.ToString(), newKeys);
                _configurationService.SetValue(valuesListKey, keysString);

                var valueKey = GetSectionValueKey(section, key);
                _configurationService.SetValue(valueKey, string.Empty);
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
                var sectionListKey = GetSectionListKey();
                var sectionsString = _configurationService.GetValue<string>(sectionListKey);
                if (string.IsNullOrEmpty(sectionsString))
                {
                    return true;
                }

                var newSections = sectionsString.Split(Separator).Where(x => !string.Equals(x, section));
                sectionsString = string.Join(Separator.ToString(), newSections);
                _configurationService.SetValue(sectionListKey, sectionsString);

                var values = GetValues(section, false);
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

            return result;
        }

        private void SetNuGetValues(string section, IList<KeyValuePair<string, string>> values)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            EnsureSectionExists(section);

            var valuesListKey = GetSectionValuesListKey(section);
            UpdateKeysList(values, valuesListKey);
            foreach (var keyValuePair in values)
            {
                SetNuGetValue(section, keyValuePair.Key, keyValuePair.Value);
            }
        }

        private void EnsureSectionExists(string section)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            var sectionListKey = GetSectionListKey();
            var sectionsString = _configurationService.GetValue(sectionListKey, string.Empty);
            var sections = sectionsString.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!sections.Contains(section))
            {
                sections.Add(section);
                sectionsString = string.Join(Separator.ToString(), sections);
                _configurationService.SetValue(sectionListKey, sectionsString);
            }
        }

        private void SetNuGetValues(string section, string subsection, IList<KeyValuePair<string, string>> values)
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

        private void UpdateKeysList(IList<KeyValuePair<string, string>> values, string valuesListKey)
        {
            var valueKeysString = _configurationService.GetValue<string>(valuesListKey);

            var existedKeys = string.IsNullOrEmpty(valueKeysString) ? Enumerable.Empty<string>() : valueKeysString.Split(Separator);
            var keysToSave = values.Select(x => x.Key);

            var newValueKeysString = string.Join(Separator.ToString(), existedKeys.Union(keysToSave));
            _configurationService.SetValue(valuesListKey, newValueKeysString);
        }

        private string ConvertToFullPath(string result)
        {
            return result;
        }

        private IList<SettingValue> GetNuGetValues(string section, bool isPath = false)
        {
            Argument.IsNotNullOrWhitespace(() => section);

            var valuesListKey = GetSectionValuesListKey(section);
            var valueKeysString = _configurationService.GetValue<string>(valuesListKey);
            if (string.IsNullOrEmpty(valueKeysString))
            {
                return null;
            }
            var keys = valueKeysString.Split(Separator);

            return keys.Select(key => GetNuGetValue(section, key, isPath)).ToList();
        }

        private IList<SettingValue> GetNuGetValues(string section, string subsection, bool isPath = false)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);

            var valuesListKey = GetSubsectionValuesListKey(section, subsection);
            var valueKeysString = _configurationService.GetValue<string>(valuesListKey);
            if (string.IsNullOrEmpty(valueKeysString))
            {
                return null;
            }

            var keys = valueKeysString.Split(Separator);

            return keys.Select(key => GetNuGetValue(section, subsection, key, isPath)).ToList();
        }

        private SettingValue GetNuGetValue(string section, string key, bool isPath)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSectionValueKey(section, key);
            var value = _configurationService.GetValue<string>(combinedKey);

            if (isPath)
            {
                value = ConvertToFullPath(value);
            }

            return new SettingValue(key, value, false);
        }

        private SettingValue GetNuGetValue(string section, string subsection, string key, bool isPath)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSubsectionValueKey(section, subsection, key);
            var value = _configurationService.GetValue<string>(combinedKey);

            if (isPath)
            {
                value = ConvertToFullPath(value);
            }

            return new SettingValue(key, value, false);
        }

        private void SetNuGetValue(string section, string key, string value)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSectionValueKey(section, key);
            _configurationService.SetValue(combinedKey, value);
        }

        private void SetNuGetValue(string section, string subsection, string key, string value)
        {
            Argument.IsNotNullOrWhitespace(() => section);
            Argument.IsNotNullOrWhitespace(() => subsection);
            Argument.IsNotNullOrWhitespace(() => key);

            var combinedKey = GetSubsectionValueKey(section, subsection, key);
            _configurationService.SetValue(combinedKey, value);
        }

        private string GetSectionValueKey(string section, string key)
        {
            return string.Format("NuGet_{0}_value_{1}", section, key);
        }

        private string GetSubsectionValueKey(string section, string subsection, string key)
        {
            return string.Format("NuGet_{0}_{1}_value_{2}", section, subsection, key);
        }

        private static string GetSectionValuesListKey(string section)
        {
            return string.Format("NuGet_{0}_values", section);
        }

        private static string GetSubsectionValuesListKey(string section, string subsection)
        {
            return string.Format("NuGet_{0}_{1}_values", section, subsection);
        }

        private static string GetSectionListKey()
        {
            return "NuGet_sections";
        }
        #endregion
    }
}