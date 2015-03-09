// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetSettings.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
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
            var settingValue = GetValues(section, isPath).FirstOrDefault(x => string.Equals(x.Key, key));

            var result = settingValue == null ? string.Empty : settingValue.Value;

            return result;
        }

        public IList<SettingValue> GetValues(string section, bool isPath)
        {
            return GetNuGetValues(section, section, isPath);
        }

        public IList<SettingValue> GetNestedValues(string section, string subsection)
        {
            return GetNuGetValues(section);
        }

        public void SetValue(string section, string key, string value)
        {
            SetNuGetValues(section, new[] {new KeyValuePair<string, string>(key, value)});
        }

        public void SetValues(string section, IList<KeyValuePair<string, string>> values)
        {
            SetNuGetValues(section, values);
        }

        public void SetNestedValues(string section, string key, IList<KeyValuePair<string, string>> values)
        {
            SetNuGetValues(section, key, values);
        }

        public bool DeleteValue(string section, string key)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteSection(string section)
        {
            throw new System.NotImplementedException();
        }

        private void SetNuGetValues(string section, IList<KeyValuePair<string, string>> values)
        {
            var valuesListKey = GetSectionValuesListKey(section);
            UpdateKeysList(values, valuesListKey);
            foreach (var keyValuePair in values)
            {
                SetNuGetValue(section, keyValuePair.Key, keyValuePair.Value);
            }
        }

        private void SetNuGetValues(string section, string subsection, IList<KeyValuePair<string, string>> values)
        {
            var valuesListKey = GetSubsectionValuesListKey(section, subsection);
            UpdateKeysList(values, valuesListKey);
            foreach (var keyValuePair in values)
            {
                SetNuGetValue(section, subsection, keyValuePair.Key, keyValuePair.Value);
            }
        }

        private void UpdateKeysList(IList<KeyValuePair<string, string>> values, string valuesListKey)
        {
            var valueKeysString = _configurationService.GetValue(valuesListKey, string.Empty);

            var existedKeys = valueKeysString.Split(Separator);
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
            var valuesListKey = GetSectionValuesListKey(section);
            var valueKeysString = _configurationService.GetValue(valuesListKey, string.Empty);
            var keys = valueKeysString.Split(Separator);

            return keys.Select(key => GetNuGetValue(section, key, isPath)).ToList();
        }

        private IList<SettingValue> GetNuGetValues(string section, string subsection, bool isPath = false)
        {
            var valuesListKey = GetSubsectionValuesListKey(section, subsection);
            var valueKeysString = _configurationService.GetValue(valuesListKey, string.Empty);
            var keys = valueKeysString.Split(Separator);

            return keys.Select(key => GetNuGetValue(section, subsection, key, isPath)).ToList();
        }

        private SettingValue GetNuGetValue(string section, string key, bool isPath)
        {
            var combinedKey = GetSectionValueKey(section, key);
            var value = _configurationService.GetValue(combinedKey, string.Empty);

            if (isPath)
            {
                value = ConvertToFullPath(value);
            }

            return new SettingValue(key, value, false);
        }

        private SettingValue GetNuGetValue(string section, string subsection, string key, bool isPath)
        {
            var combinedKey = GetSubsectionValueKey(section, subsection, key);
            var value = _configurationService.GetValue(combinedKey, string.Empty);

            if (isPath)
            {
                value = ConvertToFullPath(value);
            }

            return new SettingValue(key, value, false);
        }

        private void SetNuGetValue(string section, string key, string value)
        {
            var combinedKey = GetSectionValueKey(section, key);
            _configurationService.SetValue(combinedKey, value);
        }

        private void SetNuGetValue(string section, string subsection, string key, string value)
        {
            var combinedKey = GetSubsectionValueKey(section, subsection, key);
            _configurationService.SetValue(combinedKey, value);
        }

        private string GetSectionValueKey(string section, string key)
        {
            return string.Format("NuGetSection_{0}_value_{1}", section, key);
        }

        private string GetSubsectionValueKey(string section, string subsection, string key)
        {
            return string.Format("NuGetSection_{0}_subsection_{1}_value_{2}", section, subsection, key);
        }

        private static string GetSectionValuesListKey(string section)
        {
            return string.Format("NuGetSection_{0}_values", section);
        }

        private static string GetSubsectionValuesListKey(string section, string subsection)
        {
            return string.Format("NuGetSection_{0}_subsection_{1}_values", section, subsection);
        }
        #endregion
    }
}