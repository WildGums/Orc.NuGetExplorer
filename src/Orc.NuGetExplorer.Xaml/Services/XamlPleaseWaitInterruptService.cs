namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Services;

    public class XamlPleaseWaitInterruptService : IPleaseWaitInterruptService
    {
        #region Fields
        private readonly IPleaseWaitService _pleaseWaitService;
        #endregion

        #region Constructors
        public XamlPleaseWaitInterruptService(IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull(() => pleaseWaitService);

            _pleaseWaitService = pleaseWaitService;
        }
        #endregion

        #region Methods
        public IDisposable InterruptTemporarily()
        {
            return _pleaseWaitService.HideTemporarily();
        }
        #endregion
    }
}