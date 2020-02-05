namespace Orc.NuGetExplorer.Scenario
{
    using System.Threading.Tasks;

    public interface IUpgradeScenario
    {
        Task<bool> RunAsync();
    }
}
