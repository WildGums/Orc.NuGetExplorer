namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using Catel.Logging;
    using NuGet.Configuration;
    using Orc.NuGetExplorer.Configuration;

    internal class NuGetPackageSourceProvider : PackageSourceProvider
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly ISettings _settingsManager;

        public NuGetPackageSourceProvider(ISettings settingsManager, IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
            : base(settingsManager, defaultPackageSourcesProvider.GetDefaultPackages().ToPackageSourceInstances())
        {
            _settingsManager = settingsManager;
        }

        public void SortPackageSources(List<string> packageSourceNames)
        {
            if (_settingsManager is NuGetSettings nugetSettings)
            {
                nugetSettings.UpdatePackageSourcesKeyListSorting(packageSourceNames);
            }
            else
            {
                Log.Debug($"Sorting operation for NuGet Settings source of type {_settingsManager.GetType()} is not implemented");
            }
        }
    }
}
