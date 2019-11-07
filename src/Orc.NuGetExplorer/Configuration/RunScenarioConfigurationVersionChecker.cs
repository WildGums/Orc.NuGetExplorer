namespace Orc.NuGetExplorer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Catel.Logging;
    using NuGet.Configuration;
    using Orc.NuGetExplorer.Scenario;

    public class RunScenarioConfigurationVersionChecker : ConfigurationVersionCheckerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IVersionedSettings _settings;

        private readonly ICollection<IUpgradeScenario> _runOnCheckList = new List<IUpgradeScenario>();

        public RunScenarioConfigurationVersionChecker(ISettings settings) : base(settings)
        {
            _settings = (settings as IVersionedSettings);
        }

        public override void Check()
        {
            base.Check();

            Log.Info("Current configuration version does not match for NuGetExplorer version");
            Log.Info("Check is current configuration version older..");

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            if (currentVersion.CompareTo(_settings.Version) > 0)
            {
                foreach (var scenario in _runOnCheckList)
                {
                    Log.Info($"Run {scenario.GetType().Name}");
                    scenario.Run();

                    //update config version
                    _settings.UpdateVersion();
                }

                RaiseUpdated(new EventArgs());
            }
            else
            { 
                Log.Info("Current configuration version is higher than runned NuGetExplorer version");
                Log.Info("Check compatibility..");

                if(_settings.MinimalVersion > Assembly.GetExecutingAssembly().GetName().Version)
                {
                    throw new ApplicationException("This version of NuGetExplorer does not supported current configuration");
                }
            }
        }

        public void AddUpgradeScenario(IUpgradeScenario scenario)
        {
            _runOnCheckList.Add(scenario);
        }
    }
}
