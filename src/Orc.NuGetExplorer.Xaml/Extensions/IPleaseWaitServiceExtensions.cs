namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.Services;

    public static class IPleaseWaitServiceExtensions
    {
        public static IDisposable WaitingScope(this IPleaseWaitService pleaseWaitService)
        {
            return new DisposableToken<IPleaseWaitService>(pleaseWaitService, token => token.Instance.Push(), token => token.Instance.Pop());
        }
    }
}
