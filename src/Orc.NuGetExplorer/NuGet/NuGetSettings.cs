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
          //  var settingValue = GetSettingValue(section, key);
            return settingValue.Value;
        }

        public IList<SettingValue> GetValues(string section, bool isPath)
        {
            var valuesListKey = GetSectionValuesListKey(section);
            return GetValues(section, valuesListKey);
        }

        private SettingValue GetSettingValue(string section, string key)
        {
            var combinedKey = GetSectionValueKey(section, key);
            var value = _configurationService.GetValue(combinedKey, string.Empty);
            return new SettingValue(key, value, false);
        }

        public IList<SettingValue> GetNestedValues(string section, string subsection)
        {
            var valuesListKey = GetSubsectionValuesListKey(section, subsection);
            return GetValues(section, valuesListKey);
        }

        private IList<SettingValue> GetValues(string section, string valuesListKey)
        {
            var valueKeysString = _configurationService.GetValue(valuesListKey, string.Empty);
            var keys = valueKeysString.Split('|');

            return keys.Select(key => GetSettingValue(section, key)).ToList();
        }

        public void SetValue(string section, string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public void SetValues(string section, IList<KeyValuePair<string, string>> values)
        {
            throw new System.NotImplementedException();
        }

        public void SetNestedValues(string section, string key, IList<KeyValuePair<string, string>> values)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteValue(string section, string key)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteSection(string section)
        {
            throw new System.NotImplementedException();
        }

        private string GetSectionValueKey(string section, string key)
        {
            return string.Format("NuGetSection_{0}_value_{1}", section, key);
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