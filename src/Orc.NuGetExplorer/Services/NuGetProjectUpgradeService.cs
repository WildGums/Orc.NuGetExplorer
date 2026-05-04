namespace Orc.NuGetExplorer.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Catel;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using Orc.NuGetExplorer.Configuration;
using Orc.NuGetExplorer.Scenario;

internal class NuGetProjectUpgradeService : INuGetProjectUpgradeService
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(NuGetProjectUpgradeService));

    private readonly IVersionedSettings _settings;

    private readonly ICollection<IUpgradeScenario> _runOnCheckList = new List<IUpgradeScenario>();

    public NuGetProjectUpgradeService(ISettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Argument.IsOfType(nameof(settings), settings, typeof(IVersionedSettings));

        if (settings is not IVersionedSettings versionedSettings)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>($"Current configuration should have defined versions for controlling {nameof(NuGetProjectUpgradeService)}");
        }

        _settings = versionedSettings;
    }

    public event EventHandler? UpgradeStart;
    public event EventHandler? UpgradeEnd;

    public async Task<bool> CheckCurrentConfigurationAndRunAsync()
    {
        if (_settings.IsLastVersion)
        {
            return false;
        }

        Logger.LogInformation("Current configuration version does not match for configuration version");
        Logger.LogInformation("Check is current configuration version older..");

        if (!_runOnCheckList.Any())
        {
            Logger.LogInformation("No registered scenarios for upgrade");
            return false;
        }

        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

        if (currentVersion is not null && currentVersion.CompareTo(_settings.Version) > 0)
        {
            var anyCompleted = false;

            foreach (var scenario in _runOnCheckList)
            {
                Logger.LogInformation("Run {Scenario}...", scenario);
                var result = await scenario.RunAsync();
                Logger.LogInformation("Completed, returned status {Result}", result);

                anyCompleted = anyCompleted || result;
            }

            // update config version even if operation wasn't successful
            _settings.UpdateVersion();
            _settings.UpdateMinimalVersion();

            RaiseUpgradeEnd(new EventArgs());

            return anyCompleted;
        }
        else
        {
            Logger.LogInformation("Current configuration version is higher than current NuGetExplorer version");
            Logger.LogInformation("Check compatibility..");

            if (_settings.MinimalVersion > Assembly.GetExecutingAssembly().GetName().Version)
            {
                //use this if you want to break up compatibility with old versions
                //throw new ApplicationException("This version of NuGetExplorer does not supported current configuration");
            }

            return false;
        }
    }

    public void AddUpgradeScenario(IUpgradeScenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        _runOnCheckList.Add(scenario);
    }

    protected void RaiseUpgradeEnd(EventArgs e)
    {
        UpgradeEnd?.Invoke(this, e);
    }

    protected void RaiseUpgradeStart(EventArgs e)
    {
        UpgradeStart?.Invoke(this, e);
    }
}
