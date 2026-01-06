namespace Orc.NuGetExplorer;

using System;
using Catel.Services;

public class XamlPleaseWaitInterruptService : IBusyIndicatorInterruptService
{
    private readonly IBusyIndicatorService _busyIndicatorService;

    public XamlPleaseWaitInterruptService(IBusyIndicatorService busyIndicatorService)
    {
        ArgumentNullException.ThrowIfNull(busyIndicatorService);

        _busyIndicatorService = busyIndicatorService;
    }

    public IDisposable InterruptTemporarily()
    {
        return _busyIndicatorService.HideTemporarily();
    }
}