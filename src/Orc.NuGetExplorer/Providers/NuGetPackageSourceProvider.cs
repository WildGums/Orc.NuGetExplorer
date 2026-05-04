namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using Orc.NuGetExplorer.Configuration;

internal class NuGetPackageSourceProvider : PackageSourceProvider
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(NuGetPackageSourceProvider));

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
            Logger.LogDebug("Sorting operation for NuGet Settings source of type {SettingsManagerType} is not implemented", _settingsManager.GetType());
        }
    }
}
