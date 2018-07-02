// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiPackageRegistry.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Services;
    using NuGet;

    internal sealed class ApiPackageRegistry : IApiPackageRegistry
    {
        #region Constructors
        public ApiPackageRegistry(ILanguageService languageService)
        {
            Argument.IsNotNull(() => languageService);

            _languageService = languageService;
        }
        #endregion

        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, SemanticVersion> _apiPackages = new Dictionary<string, SemanticVersion>();

        private readonly ILanguageService _languageService;

        private readonly object _syncObj = new object();
        #endregion

        #region Methods
        public void Register(string packageName, string version)
        {
            Argument.IsNotNullOrWhitespace(() => packageName);

            var semanticVersion = SemanticVersion.Parse(version);

            lock (_syncObj)
            {
                if (_apiPackages.TryGetValue(packageName, out var storedSemanticVersion))
                {
                    throw Log.ErrorAndCreateException<ArgumentException>("The api package '{0}' is already registered with version '{1}'", packageName, storedSemanticVersion);
                }

                _apiPackages.Add(packageName, semanticVersion);
            }
        }

        public bool IsRegistered(string packageName)
        {
            lock (_syncObj)
            {
                return _apiPackages.ContainsKey(packageName);
            }
        }

        public void Validate(IPackageDetails package)
        {
            Argument.IsNotNull(() => package);
            Argument.IsOfType(() => package, typeof(PackageDetails));

            var innerPackage = ((PackageDetails)package).Package;

            lock (_syncObj)
            {
                foreach (var dependencySet in innerPackage.DependencySets)
                {
                    foreach (var dependency in dependencySet.Dependencies)
                    {
                        ValidateDependency(package, dependency);
                    }
                }
            }
        }

        private void ValidateDependency(IPackageDetails package, PackageDependency dependency)
        {
            SemanticVersion currentVersion;

            lock (_syncObj)
            {
                if (!_apiPackages.TryGetValue(dependency.Id, out currentVersion))
                {
                    return;
                }
            }

            var versionSpec = dependency.VersionSpec;

            var minVersion = versionSpec.MinVersion;
            if (minVersion != null)
            {
                if (versionSpec.IsMinInclusive && currentVersion < minVersion)
                {
                    package.ValidationContext.Add(BusinessRuleValidationResult.CreateErrorWithTag(string.Format(_languageService.GetString("NuGetExplorer_ApiPackageRegistry_Validation_Error_Message_Pattern_1"), package.Id, dependency.Id, dependency.VersionSpec.MinVersion, currentVersion), ValidationTags.Api));
                }

                if (!versionSpec.IsMinInclusive && currentVersion <= minVersion)
                {
                    package.ValidationContext.Add(BusinessRuleValidationResult.CreateErrorWithTag(string.Format(_languageService.GetString("NuGetExplorer_ApiPackageRegistry_Validation_Error_Message_Pattern_2"), package.Id, dependency.Id, dependency.VersionSpec.MinVersion, currentVersion), ValidationTags.Api));
                }
            }

            var maxVersion = versionSpec.MaxVersion;
            if (maxVersion == null)
            {
                return;
            }

            if (versionSpec.IsMaxInclusive && currentVersion > maxVersion)
            {
                package.ValidationContext.Add(BusinessRuleValidationResult.CreateErrorWithTag(string.Format(_languageService.GetString("NuGetExplorer_ApiPackageRegistry_Validation_Error_Message_Pattern_3"), package.Id, dependency.Id, dependency.VersionSpec.MaxVersion, currentVersion), ValidationTags.Api));
            }

            if (!versionSpec.IsMaxInclusive && currentVersion >= maxVersion)
            {
                package.ValidationContext.Add(BusinessRuleValidationResult.CreateErrorWithTag(string.Format(_languageService.GetString("NuGetExplorer_ApiPackageRegistry_Validation_Error_Message_Pattern_4"), package.Id, dependency.Id, dependency.VersionSpec.MaxVersion, currentVersion), ValidationTags.Api));
            }
        }
        #endregion
    }
}
