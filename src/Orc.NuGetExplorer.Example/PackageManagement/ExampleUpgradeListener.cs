namespace Orc.NuGetExplorer.Example;

using System;
using Catel.IoC;
using Catel.Services;
using Orc.NuGetExplorer.Scenario;
using Orc.NuGetExplorer.Services;

internal class ExampleUpgradeListener : UpgradeListenerBase, IConstructAtStartup
{
    private readonly ILanguageService _languageService;
    private readonly IMessageService _messageService;

    public ExampleUpgradeListener(INuGetProjectUpgradeService upgradeService, ILanguageService languageService, IMessageService messageService)
        : base(upgradeService)
    {
        ArgumentNullException.ThrowIfNull(languageService);
        ArgumentNullException.ThrowIfNull(messageService);

        _languageService = languageService;
        _messageService = messageService;
    }

    protected override void OnUpgraded(object sender, EventArgs e)
    {
        _messageService.ShowAsync(_languageService.GetRequiredString("NuGetExplorerExample_ExampleUpgradeListener_Message_Upgraded"));
    }

    protected override void OnUpgrading(object sender, EventArgs e)
    {
        _messageService.ShowAsync(_languageService.GetRequiredString("NuGetExplorerExample_ExampleUpgradeListener_Message_Upgrading"));
    }
}
