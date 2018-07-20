// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetConfigurationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using Catel.IO;
    using MethodTimer;
    using NuGet;

    internal class NuGetConfigurationService : INuGetConfigurationService
    {
        #region Fields
        private readonly IConfigurationService _configurationService;
        private readonly string _defaultDestinationFolder;
        private readonly INuGetFeedVerificationService _feedVerificationService;
        private readonly IPackageSourceProvider _packageSourceProvider;
        #endregion

        #region Constructors
        public NuGetConfigurationService(IConfigurationService configurationService, IPackageSourceProvider packageSourceProvider,
            INuGetFeedVerificationService feedVerificationService)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => packageSourceProvider);
            Argument.IsNotNull(() => feedVerificationService);

            _configurationService = configurationService;
            _packageSourceProvider = packageSourceProvider;
            _feedVerificationService = feedVerificationService;

            var applicationDataDirectory = Path.GetApplicationDataDirectory();
            _defaultDestinationFolder = Path.Combine(applicationDataDirectory, "plugins");
        }
        #endregion

        #region Methods
        public string GetDestinationFolder()
        {
            return _configurationService.GetRoamingValue(Settings.NuGet.DestinationFolder, _defaultDestinationFolder);
        }

        public void SetDestinationFolder(string value)
        {
            Argument.IsNotNullOrWhitespace(() => value);

            _configurationService.SetRoamingValue(Settings.NuGet.DestinationFolder, value);
        }

        public IEnumerable<IPackageSource> LoadPackageSources(bool onlyEnabled = false)
        {
            var packageSources = _packageSourceProvider.LoadPackageSources();

            if (onlyEnabled)
            {
                packageSources = packageSources.Where(x => x.IsEnabled);
            }

            return packageSources.ToPackageSourceInterfaces();
        }

        [Time]
        public bool SavePackageSource(string name, string source, bool isEnabled = true, bool isOfficial = true, bool verifyFeed = true)
        {
            Argument.IsNotNullOrWhitespace(() => name);
            Argument.IsNotNullOrWhitespace(() => source);

            try
            {
                if (verifyFeed)
                {
                    var verificationResult = _feedVerificationService.VerifyFeed(source, false);
                    if (verificationResult == FeedVerificationResult.Invalid || verificationResult == FeedVerificationResult.Unknown)
                    {
                        return false;
                    }
                }

                var packageSources = _packageSourceProvider.LoadPackageSources().ToList();

                var existedSource = packageSources.FirstOrDefault(x => string.Equals(x.Name, name));
                if (existedSource == null)
                {
                    existedSource = new PackageSource(source, name);
                    packageSources.Add(existedSource);
                }

                existedSource.IsEnabled = isEnabled;
                existedSource.IsOfficial = isOfficial;

                _packageSourceProvider.SavePackageSources(packageSources);
            }
            catch
            {
                return false;
            }

            return true;
        }

        [Time]
        public void SavePackageSources(IEnumerable<IPackageSource> packageSources)
        {
            Argument.IsNotNull(() => packageSources);

            _packageSourceProvider.SavePackageSources(packageSources.Cast<PackageSource>());
        }

        [Obsolete("Use DisablePackageSource")]
        public void DeletePackageSource(string name, string source)
        {
            DisablePackageSource(name, source);
        }

        public void DisablePackageSource(string name, string source)
        {
            Argument.IsNotNullOrWhitespace(() => name);

            _packageSourceProvider.DisablePackageSource(new PackageSource(source, name));
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
        #endregion
    }
}
