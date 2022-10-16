namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Services;

    public class XamlPleaseWaitInterruptService : IPleaseWaitInterruptService
    {
        private readonly IBusyIndicatorService _busyIndicatorService;

        public XamlPleaseWaitInterruptService(IBusyIndicatorService busyIndicatorService)
        {
            Argument.IsNotNull(() => busyIndicatorService);

            _busyIndicatorService = busyIndicatorService;
        }

        public IDisposable InterruptTemporarily()
        {
            return _busyIndicatorService.HideTemporarily();
        }
    }
}
