// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiPackageRegistry.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using Catel.Logging;
    using NuGet;

    internal sealed class ApiPackageRegistry : IApiPackageRegistry
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, SemanticVersion> _apiPackages = new Dictionary<string, SemanticVersion>();

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
                return _apiPackages.TryGetValue(packageName, out var _);
            }
        }

        public void Validate(IPackageDetails package)
        {
            package.ApiValidations.Clear();
            var innerPackage = ((PackageDetails) package).Package;
            foreach (var dependencySet in innerPackage.DependencySets)
            {
                foreach (var dependency in dependencySet.Dependencies)
                {
                    lock (_syncObj)
                    {
                        if (_apiPackages.TryGetValue(dependency.Id, out var currentVersion))
                        {
                            if (dependency.VersionSpec.IsMinInclusive && currentVersion < dependency.VersionSpec.MinVersion)
                            {
                                package.ApiValidations.Add($"The package '{package.Id}' depends on API '{dependency.Id}' min version '{dependency.VersionSpec.MinVersion}' but the installed version is '{currentVersion}'");
                            }

                            if (!dependency.VersionSpec.IsMinInclusive && currentVersion <= dependency.VersionSpec.MinVersion)
                            {
                                package.ApiValidations.Add($"The package '{package.Id}' depends on API '{dependency.Id}' min version greater than '{dependency.VersionSpec.MinVersion}' but the installed version is '{currentVersion}'");
                            }

                            if (dependency.VersionSpec.IsMaxInclusive && currentVersion > dependency.VersionSpec.MaxVersion)
                            {
                                package.ApiValidations.Add($"The package '{package.Id}' depends on API '{dependency.Id}' max version '{dependency.VersionSpec.MaxVersion}' but the installed version is '{currentVersion}'");
                            }

                            if (!dependency.VersionSpec.IsMaxInclusive && currentVersion >= dependency.VersionSpec.MaxVersion)
                            {
                                package.ApiValidations.Add($"The package '{package.Id}' depends on API '{dependency.Id}' max version lower than '{dependency.VersionSpec.MaxVersion}' but the installed version is '{currentVersion}'");
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}