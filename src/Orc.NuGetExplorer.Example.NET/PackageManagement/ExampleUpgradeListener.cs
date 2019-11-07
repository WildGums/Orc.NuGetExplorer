namespace Orc.NuGetExplorer.Example.PackageManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using Orc.NuGetExplorer.Configuration;
    using Orc.NuGetExplorer.Scenario;

    internal class ExampleUpgradeListener : UpgradeListenerBase
    {
        private readonly IMessageService _messageService;

        public ExampleUpgradeListener(RunScenarioConfigurationVersionChecker upgradeRunner, IMessageService messageService) : base(upgradeRunner)
        {
            Argument.IsNotNull(() => messageService);
            _messageService = messageService;
        }

        protected override void OnUpdated(object sender, EventArgs e)
        {
            _messageService.ShowAsync("Updating NuGet data");
        }

        protected override void OnUpdating(object sender, EventArgs e)
        {
            _messageService.ShowAsync("NuGet data updated");
        }
    }
}
