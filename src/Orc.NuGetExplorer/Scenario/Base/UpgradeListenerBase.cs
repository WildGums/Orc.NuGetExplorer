namespace Orc.NuGetExplorer.Scenario
{
    using System;
    using Catel;
    using Orc.NuGetExplorer.Configuration;

    public abstract class UpgradeListenerBase
    {
        protected UpgradeListenerBase(RunScenarioConfigurationVersionChecker upgradeRunner)
        {
            Argument.IsNotNull(() => upgradeRunner);

            upgradeRunner.Updated += OnUpdated;
            upgradeRunner.Updating += OnUpdating;
        }

        protected virtual void OnUpdating(object sender, EventArgs e)
        {

        }

        protected virtual void OnUpdated(object sender, EventArgs e)
        {

        }
    }
}
