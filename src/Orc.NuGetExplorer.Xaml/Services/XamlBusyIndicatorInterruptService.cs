namespace Orc.NuGetExplorer;

using System;
using Catel.Services;

public class XamlBusyIndicatorInterruptService : IBusyIndicatorInterruptService
{
    private readonly IBusyIndicatorService _busyIndicatorService;

    public XamlBusyIndicatorInterruptService(IBusyIndicatorService busyIndicatorService)
    {
        ArgumentNullException.ThrowIfNull(busyIndicatorService);

        _busyIndicatorService = busyIndicatorService;
    }

    public IDisposable InterruptTemporarily()
    {
        return _busyIndicatorService.HideTemporarily();
    }
}
