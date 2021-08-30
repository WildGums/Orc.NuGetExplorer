namespace Orc.NuGetExplorer.Scenario
{
    using System;
    using System.Threading.Tasks;

    public interface IUpgradeScenario
    {
        Task<bool> RunAsync();
        Version MaxVersion { get; }
    }
}
