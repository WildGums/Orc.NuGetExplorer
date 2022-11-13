namespace Orc.NuGetExplorer.Scenario
{
    using System;
    using Orc.NuGetExplorer.Services;

    public abstract class UpgradeListenerBase
    {
        protected UpgradeListenerBase(INuGetProjectUpgradeService upgradeRunner)
        {
            ArgumentNullException.ThrowIfNull(upgradeRunner);

            upgradeRunner.UpgradeEnd += OnUpgraded;
            upgradeRunner.UpgradeStart += OnUpgrading;
        }

        protected virtual void OnUpgrading(object? sender, EventArgs e)
        {

        }

        protected virtual void OnUpgraded(object? sender, EventArgs e)
        {

        }
    }
}
