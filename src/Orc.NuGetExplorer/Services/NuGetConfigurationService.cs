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
    using NuGet;

    internal class NuGetConfigurationService : INuGetConfigurationService
    {
        #region Fields
        private const char Separator = '|';
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
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
            return _configurationService.GetValue(Settings.DestFolder, _defaultDestinationFolder);
        }

        public void SetDestinationFolder(string value)
        {
            Argument.IsNotNullOrWhitespace(() => value);

            _configurationService.SetValue(Settings.DestFolder, value);
        }

        public IEnumerable<IPackageSource> LoadPackageSources()
        {
            return _packageSourceProvider.LoadPackageSources().ToPackageSourceInterfaces();
        }

        public bool SavePackageSource(string name, string source, bool isEnabled = true, bool isOfficial = true)
        {
            Argument.IsNotNullOrWhitespace(() => name);
            Argument.IsNotNullOrWhitespace(() => source);

            try
            {
                var verificationResult = _feedVerificationService.VerifyFeed(source);
                if (verificationResult == FeedVerificationResult.Invalid || verificationResult == FeedVerificationResult.Unknown)
                {
                    return false;
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

        public void DeletePackageSource(string name, string source)
        {
            Argument.IsNotNullOrWhitespace(() => name);

            _packageSourceProvider.DisablePackageSource(new PackageSource(source, name));
        }
        #endregion
    }
}