namespace Orc.NuGetExplorer.Example.PackageManagement
{
    using System;
    using Catel.Services;
    using Orc.NuGetExplorer.Scenario;
    using Orc.NuGetExplorer.Services;

    internal class ExampleUpgradeListener : UpgradeListenerBase
    {
        private readonly IMessageService _messageService;

        public ExampleUpgradeListener(INuGetProjectUpgradeService upgradeService, IMessageService messageService) : base(upgradeService)
        {
            _messageService = messageService;
        }

        protected override void OnUpgraded(object sender, EventArgs e)
        {
            _messageService.ShowAsync("NuGet data updated");
        }

        protected override void OnUpgrading(object sender, EventArgs e)
        {
            _messageService.ShowAsync("Updating NuGet data");
        }
    }
}
