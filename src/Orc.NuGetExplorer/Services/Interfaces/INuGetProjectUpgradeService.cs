namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Threading.Tasks;
    using Orc.NuGetExplorer.Scenario;

    public interface INuGetProjectUpgradeService
    {
        event EventHandler UpgradeStart;
        event EventHandler UpgradeEnd;

        void AddUpgradeScenario(IUpgradeScenario scenario);
        Task<bool> CheckCurrentConfigurationAndRunAsync();
    }
}
