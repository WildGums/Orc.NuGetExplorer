namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Services;

    public class XamlPleaseWaitInterruptService : IPleaseWaitInterruptService
    {
        private readonly IPleaseWaitService _pleaseWaitService;

        public XamlPleaseWaitInterruptService(IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull(() => pleaseWaitService);

            _pleaseWaitService = pleaseWaitService;
        }

        public IDisposable InterruptTemporarily()
        {
            return _pleaseWaitService.HideTemporarily();
        }
    }
}
