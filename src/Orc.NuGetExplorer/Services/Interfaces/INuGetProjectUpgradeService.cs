namespace Orc.NuGetExplorer.Services
{
    using System.Threading.Tasks;
    using Orc.NuGetExplorer.Scenario;

    public interface INuGetProjectUpgradeService
    {
        void AddUpgradeScenario(IUpgradeScenario scenario);
        Task<bool> CheckCurrentConfigurationAndRunAsync();
    }
}
