// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetConfigurationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using Catel.IO;
    using Catel.Logging;

    internal class NuGetConfigurationService : INuGetConfigurationService
    {
        #region Fields
        private const char Separator = '|';
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IConfigurationService _configurationService;
        private readonly string _defaultDestinationFolder;
        #endregion

        #region Constructors
        public NuGetConfigurationService(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;

            var applicationDataDirectory = Path.GetApplicationDataDirectory();
            _defaultDestinationFolder = Path.Combine(applicationDataDirectory, "plugins");
        }
        #endregion

        #region Methods
        public string GetDestinationFolder()
        {
            return _configurationService.GetValue(Settings.DestFolder, _defaultDestinationFolder);
        }

        public void SetDestinationFolder(string value)
        {
            Argument.IsNotNullOrWhitespace(() => value);

            _configurationService.SetValue(Settings.DestFolder, value);
        }

        public IEnumerable<IPackageSource> LoadPackageSources()
        {
            Log.Debug("Loading package sources");

            var packageSourceNames = LoadPackageSourceNames();
            var result = new List<IPackageSource>();

            foreach (var sourceName in packageSourceNames)
            {
                var sourceKey = sourceName.ToPackageSourceKey();
                var sourceValue = _configurationService.GetValue(sourceKey, string.Empty);
                if (string.IsNullOrWhiteSpace(sourceValue))
                {
                    Log.Warning("The information about package {0} was not found.", sourceName);
                    continue;
                }

                var stringValues = sourceValue.Split(Separator);
                if (stringValues.Length != 4)
                {
                    Log.Warning("The information about package {0} contains wrong amount of data. Must be 4 values separated by \'|\' (string|string|boolean|boolean).", sourceName);
                    continue;
                }

                var source = stringValues[0].Trim();
                var name = stringValues[1].Trim();
                var isEnabled = bool.Parse(stringValues[2].Trim());
                var isOfficial = bool.Parse(stringValues[3].Trim());

                var packageSource = new NuGetPackageSource(source, name, isEnabled, isOfficial);

                result.Add(packageSource);
            }

            if (!result.Any())
            {
                SavePackageSource("NuGet", "http://www.nuget.org/api/v2/");
                result.AddRange(LoadPackageSources());
            }


            Log.Debug("Package sources has been loaded");
            return result;
        }

        public void SavePackageSource(string name, string source, bool isEnabled = true, bool isOfficial = true)
        {
            Argument.IsNotNullOrWhitespace(() => name);

            var packageSourceNames = LoadPackageSourceNames().ToList();
            if (!packageSourceNames.Contains(name))
            {
                packageSourceNames.Add(name);
            }

            using (_configurationService.SuspendNotifications())
            {
                var packageSourceKey = name.ToPackageSourceKey();
                var value = string.Format("{0}|{1}|{2}|{3}", source, name, isEnabled, isOfficial);
                _configurationService.SetValue(packageSourceKey, value);
            }

            SavePackageSourceNames(packageSourceNames);
        }

        public void DeletePackageSource(string name)
        {
            Argument.IsNotNullOrWhitespace(() => name);

            var packageSourceNames = LoadPackageSourceNames().ToList();
            if (!packageSourceNames.Contains(name))
            {
                return;
            }

            using (_configurationService.SuspendNotifications())
            {
                var packageSourceKey = name.ToPackageSourceKey();
                _configurationService.SetValue(packageSourceKey, string.Empty);
            }

            SavePackageSourceNames(packageSourceNames.Where(x => !string.Equals(name, x)));
        }

        private IEnumerable<string> LoadPackageSourceNames()
        {
            var packageSourcesString = _configurationService.GetValue(Settings.PackageSources, string.Empty);
            if (string.IsNullOrWhiteSpace(packageSourcesString))
            {
                return Enumerable.Empty<string>();
            }

            var packageSourceNames = packageSourcesString.Split(Separator);
            return packageSourceNames;
        }

        private void SavePackageSourceNames(IEnumerable<string> packageSourceNames)
        {
            var packageSourcesString = string.Join(Separator.ToString(), packageSourceNames);
            _configurationService.SetValue(Settings.PackageSources, packageSourcesString);
        }
        #endregion
    }
}